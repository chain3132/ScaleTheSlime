using System.Collections.Generic;
using System.Linq;
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

        private VisualElement detail ;

        [MenuItem("Tools/ScaleTheSlime/Data Editor")]
        public static void ShowExample()
        {
            ScriptableObjectEditor wnd = GetWindow<ScriptableObjectEditor>();
            wnd.titleContent = new GUIContent("ScriptableObjectEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            var tabs = new Toolbar();
            tabs.Add(new ToolbarToggle { text = "Cards", value = true });
            root.Add(tabs);

            var split = new TwoPaneSplitView(0,250,TwoPaneSplitViewOrientation.Horizontal);
            var list = new ListView();
            detail = new VisualElement();

            split.Add(list);
            split.Add(detail);
            root.Add(split);
            split.style.flexGrow = 1;
            
            var cards = LoadAllCards();
            list.itemsSource = cards;
            list.makeItem = () => new Label();
            list.bindItem = (e, i) => ((Label)e).text = cards[i].name;
            list.selectionChanged += selected => ShowDetail(selected.FirstOrDefault() as ScriptableObject);

        }

        private List<CardDefinition> LoadAllCards()
        {
            return AssetDatabase.FindAssets("t:CardDefinition")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<CardDefinition>(path))
                .Where(a => a != null)
                .ToList();
        }
        private void ShowDetail(ScriptableObject obj)
        {
            detail.Clear();
            if(obj == null ) return;
            detail.Add(new InspectorElement(obj));
            
            
        }
    }
}
