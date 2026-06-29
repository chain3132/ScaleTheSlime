using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Card.Enum;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditor.TableView
{
    public struct CardCreationRequest
    {
        public CardDefinition Preset;
    }

    public class CardCreationWindow : EditorWindow
    {
        private const string SpriteFolder   = "Assets/Art/Sprites/UI/Gameplay";
        private const string SpriteLabel    = "cardsprite";
        private const int    SpritePickerId = 1;

        private Action<CardCreationRequest> _onSubmit;
        private CardDefinition _preset;
        private VisualElement  _body;
        private Action<Sprite> _onSpritePicked;

        public static void Open(Action<CardCreationRequest> onSubmit)
        {
            var window = CreateInstance<CardCreationWindow>();
            window._onSubmit = onSubmit;
            window.titleContent = new GUIContent("New Card");
            window.minSize = new Vector2(340, 220);
            window.ShowUtility();
        }

        public void CreateGUI()
        {
            var template = new EnumField("Template", CardEffectType.Attack);
            rootVisualElement.Add(template);

            _body = new VisualElement();
            rootVisualElement.Add(_body);

            Rebuild((CardEffectType)template.value);
            template.RegisterValueChangedCallback(e => Rebuild((CardEffectType)e.newValue));

            var create = new Button(() =>
            {
                _onSubmit?.Invoke(new CardCreationRequest { Preset = _preset });
                _preset = null;     
                Close();
            }) { text = "Create" };
            rootVisualElement.Add(create);

            rootVisualElement.Add(new IMGUIContainer(() =>
            {
                if (Event.current.commandName == "ObjectSelectorUpdated"
                    && EditorGUIUtility.GetObjectPickerControlID() == SpritePickerId)
                {
                    _onSpritePicked?.Invoke(EditorGUIUtility.GetObjectPickerObject() as Sprite);
                }
            }));
        }

        private void Rebuild(CardEffectType type)
        {
            if (_preset != null) DestroyImmediate(_preset);
            _preset = BuildPreset(type);

            _body.Clear();
            var so = new SerializedObject(_preset);

            _body.Add(new PropertyField(so.FindProperty("DisplayName")));
            _body.Add(new PropertyField(so.FindProperty("Description")));
            _body.Add(new PropertyField(so.FindProperty("PlayTarget")));
            _body.Add(new PropertyField(so.FindProperty("Keywords")));

            _body.Add(new Label("Art") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
            AddNativeSpritePicker(so, "Art.CardBackground", "Card Background");
            AddNativeSpritePicker(so, "Art.CardArt",        "Card Art");
            AddNativeSpritePicker(so, "Art.CardHeader",     "Card Header");

            _body.Add(new PropertyField(so.FindProperty("Effects")));

            _body.Bind(so);
        }

        private void AddNativeSpritePicker(SerializedObject so, string propPath, string label)
        {
            var prop = so.FindProperty(propPath);

            var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            var field = new ObjectField(label)
            {
                objectType = typeof(Sprite),
                value = prop.objectReferenceValue,
                style = { flexGrow = 1 },
            };
            field.RegisterValueChangedCallback(e =>
            {
                prop.objectReferenceValue = e.newValue;
                so.ApplyModifiedProperties();
            });
            field.Q(className: "unity-object-field__selector").style.display = DisplayStyle.None;

            var pick = new Button(() =>
            {
                _onSpritePicked = s =>
                {
                    prop.objectReferenceValue = s;
                    so.ApplyModifiedProperties();
                    field.SetValueWithoutNotify(s);
                };
                EditorGUIUtility.ShowObjectPicker<Sprite>(
                    prop.objectReferenceValue, false, $"l:{SpriteLabel} t:Sprite", SpritePickerId);
            }) { text = "◎" };

            row.Add(field);
            row.Add(pick);
            _body.Add(row);
        }

        private static CardDefinition BuildPreset(CardEffectType type)
        {
            var card = CreateInstance<CardDefinition>();
            var effect = new CardEffectData { Type = type, Repeat = 1, Target = CardTarget.Enemy };

            switch (type)
            {
                case CardEffectType.Attack:
                    card.PlayTarget  = CardTarget.Enemy;
                    effect.Target      = CardTarget.Enemy;
                    effect.ValueSource = ValueSource.Card;
                    effect.CardValue   = 5;
                    break;
                case CardEffectType.GainShield:
                   
                    card.PlayTarget  = CardTarget.Player;
                    effect.Target      = CardTarget.Player;
                    effect.ValueSource = ValueSource.Card;
                    effect.CardValue   = 3;
                    break;
                default:
                    card.PlayTarget  = CardTarget.None;
                    effect.Target    = CardTarget.None;
                    break;
            }

            card.Effects = new List<CardEffectData> { effect };
            card.Art = new CardSprites
            {
                CardBackground = LoadSprite("TX_Cards.png",      "TX_Cards_Default"),
                CardHeader     = LoadSprite("TX_Cards.png",      "TX_CardHeader_Default"),
                CardArt        = LoadSprite("TX_CardArts_1.png", "TX_CardArts_Attack"),
            };
            return card;
        }

        private static Sprite LoadSprite(string sheetFileName, string spriteName)
        {
            string path = $"{SpriteFolder}/{sheetFileName}";
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                .OfType<Sprite>()
                .FirstOrDefault(s => s.name == spriteName);
        }

        [MenuItem("Tools/ScaleTheSlime/Label Card Sprites")]
        private static void LabelCardSprites()
        {
            int count = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { SpriteFolder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var texture  = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture == null) continue;

                var labels = AssetDatabase.GetLabels(texture).ToList();
                if (labels.Contains(SpriteLabel)) continue;

                labels.Add(SpriteLabel);
                AssetDatabase.SetLabels(texture, labels.ToArray());
                count++;
            }
            AssetDatabase.SaveAssets();
        }

        private void OnDestroy()
        {
            if (_preset != null) DestroyImmediate(_preset);
        }
    }
}
