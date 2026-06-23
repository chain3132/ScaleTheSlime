using System;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Card.Enum;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditor.TableView
{
    public struct CardCreationRequest
    {
        public string DisplayName;
        public CardTarget PlayTarget;
        public CardEffectData Preset;
    }
    public class CardCreationWindow : EditorWindow
    {
        private Action<CardCreationRequest> _onSubmit;
        public static void Open(Action<CardCreationRequest> onSubmit)
        {
            var window = CreateInstance<CardCreationWindow>();
            window._onSubmit = onSubmit;
            window.titleContent = new GUIContent("New Card");
            window.minSize = new Vector2(320, 180);
            window.ShowUtility();
        }

        public void CreateGUI()
        {
            var displayName = new TextField("Display Name");
            var effect = new EnumField("Main Effect", CardEffectType.Attack);
            var target = new EnumField("Card Target", CardTarget.Enemy);
            rootVisualElement.Add(displayName);
            rootVisualElement.Add(effect);
            rootVisualElement.Add(target);

            var create = new Button(() =>
            {
                var type = (CardEffectType)effect.value;
                _onSubmit?.Invoke(new CardCreationRequest
                {
                    DisplayName = displayName.value,
                    PlayTarget = (CardTarget)target.value,
                    Preset = BuildPreset(type),
                });
                Close();

            }) { text = "Create" };
            rootVisualElement.Add(create);
        }

        private CardEffectData BuildPreset(CardEffectType type)
        {
            var e = new CardEffectData { Type = type, Repeat = 1 };
            switch (type)
            {
                case CardEffectType.Attack:
                    e.Target = CardTarget.Enemy;
                    e.ValueSource = ValueSource.Card;
                    e.CardValue = 5;
                    break;
                case CardEffectType.GainShield:
                    e.Target = CardTarget.Player;
                    e.ValueSource = ValueSource.Card;
                    e.CardValue = 3;
                    break;
                default:
                    e.Target = CardTarget.None;
                    break;
            }

            return e;
        }
    }
}
