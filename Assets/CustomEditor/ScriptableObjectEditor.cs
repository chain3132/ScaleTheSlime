using System;
using System.Collections.Generic;
using System.Linq;
using CustomEditor.TableView;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.UI.Card;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditor
{
    public class ScriptableObjectEditor : EditorWindow
    {

        private VisualElement detail;
        private VisualElement _sidebarHost;
        private readonly Dictionary<string, string[]> _definitionGroups = new()
        {
            { "Cards",      new[] { "CardDefinition" } },
            { "Characters", new[] { "PlayerDefinition", "EnemyDefinition" } },
            { "Nodes",      new[] { "NodeDefinition", } },
            { "Behaviors",  new[] { "MultiActionBehavior" } },
        };
        private readonly Dictionary<string,string[]> _databaseGroups = new()
        {
            { "Databases", new[]{ "DeckDefinition", "CardRewardPool", "NodeDatabase",
                "StatusDatabase", "ActionIntentDatabase", "CharacterFxDatabase" } },
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


            var tab = new Toolbar();
            var definitionTab = new ToolbarToggle { text = "Definitions", value = true };
            var databaseTab = new ToolbarToggle { text = "Databases" };
            tab.Add(definitionTab);
            tab.Add(databaseTab);
            rootVisualElement.Add(tab);

            var split = new TwoPaneSplitView(0,250,TwoPaneSplitViewOrientation.Horizontal);
            split.style.flexGrow = 1;
            rootVisualElement.Add(split);

            _sidebarHost = new VisualElement { style = { flexGrow = 1 } };
            detail = new VisualElement { style = { flexGrow = 1 } };
            
            split.Add(_sidebarHost);
            split.Add(detail);
            ShowTab(_definitionGroups, isDatabase: false);

            definitionTab.RegisterValueChangedCallback(e =>
            {
                if (e.newValue)
                {
                    {
                        databaseTab.SetValueWithoutNotify(false);
                        ShowTab(_definitionGroups, false);
                    }
                }
            });
            databaseTab.RegisterValueChangedCallback(e =>
            {
                if (e.newValue)
                {
                    definitionTab.SetValueWithoutNotify(false);
                    ShowTab(_databaseGroups, true);
                }
            });

        }

        #region SideBar

        private VisualElement MakeSidebar(Dictionary<string,string[]> groups, bool isDatabase)
        {
            var scroll = new ScrollView();
            scroll.AddToClassList("nav");
            foreach (var group in groups)
            {
                var foldout = new Foldout { text = group.Key, value = true };   
                foldout.AddToClassList("nav-folder");
                foreach (var typeName in group.Value)
                {
                    foldout.Add(isDatabase ? MakeDatabaseItem(typeName) : MakeTypeAsset(typeName));
                }
                scroll.Add(foldout);
            }
            return scroll;
        }
        private VisualElement MakeDatabaseItem(string typeName)
        {
            int count = AssetDatabase.FindAssets($"t:{typeName}").Length;
            var item = new Label($"{typeName} ({count})");
            item.AddToClassList("nav-item");
            item.RegisterCallback<ClickEvent>(_ => ShowDatabase(typeName));
            return item;
        }
        private void ShowDatabase(string typeName)
        {
            detail.Clear();
            var guids = AssetDatabase.FindAssets($"t:{typeName}");
            if (guids.Length == 0) { detail.Add(new Label($"No {typeName} asset")); return; }

            var scroll = new ScrollView();
            foreach (var guid in guids)
            {
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null) scroll.Add(new InspectorElement(asset));  
            }
            detail.Add(scroll);
        }
        private VisualElement MakeTypeAsset(string typeName)
        {
            int count = AssetDatabase.FindAssets($"t:{typeName}").Length;
            var item = new Label($"{typeName} ({count})");
            item.AddToClassList("nav-item");                       
            item.RegisterCallback<ClickEvent>(_ => ShowType(typeName));
            return item;
        }
        

        private void ShowType(string typeName)
        {
            detail.Clear();
            if (typeName == "CardDefinition")
                detail.Add(new CardTableView());
            else if (typeName == "NodeDefinition")
                detail.Add(new NodeTableView());
            else if (typeName == "PlayerDefinition")
            {
                detail.Add(new CharacterTableView(typeof(PlayerDefinition)));
            }
            else if (typeName == "EnemyDefinition")
            {
                detail.Add(new CharacterTableView(typeof(EnemyDefinition)));
            }
            else if (typeName == "MultiActionBehavior")
            {
                detail.Add(new MultiActionBehaviorView());
            }
            else
                detail.Add(new Label($"not available {typeName}"));
        }

        private void ShowTab(Dictionary<string, string[]> groups, bool isDatabase)
        {
            _sidebarHost.Clear();
            _sidebarHost.Add(MakeSidebar(groups,isDatabase));
            detail.Clear();
        }

        #endregion
        

        

    }
}
