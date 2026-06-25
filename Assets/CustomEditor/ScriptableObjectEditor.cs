using System;
using System.Collections.Generic;
using System.Linq;
using CustomEditor.TableView;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.UI.Card;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditor
{
    public class ScriptableObjectEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private VisualElement detail;
        private readonly Dictionary<string, string[]> _groups = new()
        {
            { "Cards",      new[] { "CardDefinition", "DeckDefinition", "CardRewardPool" } },
            { "Characters", new[] { "PlayerDefinition", "EnemyDefinition" } },
            { "Nodes",      new[] { "NodeDefinition", "NodeDatabase" } },
            { "Databases",  new[] { "StatusDatabase", "ActionIntentDatabase", "CharacterFxDatabase" } },
        };

        private readonly HashSet<string> _perAssetTypes = new ()
        {
            "EnemyDefinition"
        };
        
        [MenuItem("Tools/ScaleTheSlime/Data Editor")]
        public static void ShowExample()
        {
            ScriptableObjectEditor wnd = GetWindow<ScriptableObjectEditor>();
            wnd.titleContent = new GUIContent("ScriptableObjectEditor");
        }

        public void CreateGUI()
        {
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/CustomEditor/ScriptableObjectEditor.uss");
            rootVisualElement.styleSheets.Add(uss);
            
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            

            var split = new TwoPaneSplitView(0,250,TwoPaneSplitViewOrientation.Horizontal);
            split.style.flexGrow = 1;
            root.Add(split);
            
            split.Add(MakeSidebar());
            detail = new VisualElement();
            detail.style.flexGrow = 1;
            split.Add(detail);


        }

        #region SideBar

        private VisualElement MakeSidebar()
        {
            var scroll = new ScrollView();
            scroll.AddToClassList("nav");
            foreach (var group in _groups)
            {
                var foldout = new Foldout { text = group.Key, value = true };   
                foldout.AddToClassList("nav-folder");
                foreach (var typeName in group.Value)
                {
                    if (_perAssetTypes.Contains(typeName))
                    {
                        foldout.Add(MakeTypeWithAssets(typeName));
                    }
                    else
                    {
                        foldout.Add(MakeTypeAsset(typeName));;
                    }
                }
                scroll.Add(foldout);
            }
            return scroll;
        }

        private VisualElement MakeTypeAsset(string typeName)
        {
            int count = AssetDatabase.FindAssets($"t:{typeName}").Length;
            var item = new Label($"{typeName} ({count})");
            item.AddToClassList("nav-item");                       
            item.RegisterCallback<ClickEvent>(_ => ShowType(typeName));
            return item;
        }

        private VisualElement MakeTypeWithAssets(string typeName)
        {
            var guids = AssetDatabase.FindAssets($"t:{typeName}");
            var typeFolder = new Foldout { text = $"{typeName} ({guids.Length})",value = false};
            typeFolder.AddToClassList("nav-subFolder");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (asset == null)
                {
                    continue;
                }
                var item = new Label($"{asset.name}");
                item.AddToClassList("nav-item");
                item.AddToClassList("nav-subItem");
                item.RegisterCallback<ClickEvent>(_ => ShowAsset(asset));
                typeFolder.Add(item);
                
            }

            return typeFolder;

        }

        private void ShowAsset(ScriptableObject asset)
        {
            detail.Clear();
            if (asset is EnemyDefinition enemy)
                detail.Add(new ScrollView { } );     
            detail.Add(new EnemyTableView(asset as EnemyDefinition));
        }

        private void ShowType(string typeName)
        {
            detail.Clear();
            if (typeName == "CardDefinition")
                detail.Add(new CardTableView());
            else if (typeName == "NodeDefinition")
                detail.Add(new NodeTableView());
            else
                detail.Add(new Label($"not available {typeName}"));  }

        #endregion
        

        

    }
}
