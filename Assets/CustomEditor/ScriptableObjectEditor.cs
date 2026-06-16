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

        private ScrollView  detail ;
        private string _sortMode = "name";
        private readonly Dictionary<string, string[]> _groups = new()
        {
            { "Cards",      new[] { "CardDefinition", "DeckDefinition", "CardRewardPool" } },
            { "Characters", new[] { "PlayerDefinition", "EnemyDefinition" } },
            { "Nodes",      new[] { "NodeDefinition", "NodeDatabase" } },
            { "Databases",  new[] { "StatusDatabase", "ActionIntentDatabase", "CharacterFxDatabase" } },
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
            detail = new ScrollView();
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
                    int count = AssetDatabase.FindAssets($"t:{typeName}").Length;  
                    var item = new Label($"{typeName} ({count})");
                    item.AddToClassList("nav-item");                       
                    item.RegisterCallback<ClickEvent>(_ => ShowType(typeName));
                    foldout.Add(item);
                }
                scroll.Add(foldout);
            }
            return scroll;
        }
        private void ShowType(string typeName)
        {
            if (typeName == "CardDefinition")
                ShowCards();
            else
                ShowNotReady(typeName);     
        }
        private void ShowNotReady(string typeName)
        {
            detail.Clear();
            detail.Add(new Label($"ยังไม่รองรับ {typeName}"));
        }

        #endregion
        

        #region Cards

        private VisualElement MakeHeader()
        {
            var h = new VisualElement();
            h.AddToClassList("Header");
            foreach (var (name, w) in new[] {("Id",50),("DisplayName",120),("Art",300),("Effects",280)})
            {
                var lbl = new Label(name);
                lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
                lbl.style.width = w;
                lbl.style.height = 20;
                h.Add(lbl);
            }
            return h;
        }
        private VisualElement MakeToolbar()
        {
            var bar = new Toolbar();

            var sortMenu = new ToolbarMenu { text = "Sort" };
            sortMenu.menu.AppendAction("Id asc", _ => SetSort("id_asc"));
            sortMenu.menu.AppendAction("Id desc", _ => SetSort("id_desc"));
            sortMenu.menu.AppendAction("name a-z", _ => SetSort("name"));
            bar.Add(sortMenu);

            var create = new ToolbarButton(CreateNewCard) { text = "+ New" };
            create.AddToClassList("create-button");
            bar.Add(create);
            bar.Add(new ToolbarButton(CollapseAllCards) {text = "Collapse All"});
            return bar;
        }
        private VisualElement MakeRow(CardDefinition card ,int index)
        {
            var so = new SerializedObject(card);
            
            var row = new VisualElement();
            row.AddToClassList("row");
            if (index % 2 == 1)
                row.AddToClassList("row-odd");
            
            AddCell(row, so, "Id",          50);
            AddCell(row, so, "DisplayName", 120);
            AddCell(row, so, "Art",         300);
            AddCell(row, so, "Effects",     280);

            var deleteButton = new Button((() => DeleteCard(card))) { text = "X" };
            deleteButton.AddToClassList("delete-button");
            row.Add(deleteButton);
            row.Bind(so);           
            return row;
        }

        private void AddCell(VisualElement row, SerializedObject so, string prop, float width)
        {
            var field = new PropertyField(so.FindProperty(prop));
            field.label = "";               
            field.AddToClassList("cell"); 
            field.style.width = width;
            
            row.Add(field);

        }
        
        
        private void SetSort(string mode)
        {
            _sortMode = mode;
            ShowCards();
        }

        private List<CardDefinition> LoadAllCards()
        {
            var cards = AssetDatabase.FindAssets("t:CardDefinition")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<CardDefinition>(path))
                .Where(a => a != null);
            
            switch (_sortMode)
            {
                case "id_asc":  cards = cards.OrderBy(c => c.Id); break;
                case "id_desc": cards = cards.OrderByDescending(c => c.Id); break;
                default:        cards = cards.OrderBy(c => c.name); break;
            }
            return cards.ToList();
        }
        private void ShowCards()
        {
            detail.Clear();
            detail.Add(MakeToolbar());
            detail.Add(MakeHeader());
            
            var cards = LoadAllCards();
            for (int i = 0; i < cards.Count; i++)
                detail.Add(MakeRow(cards[i], i));
        }
        private void CreateNewCard()
        {
            var card = ScriptableObject.CreateInstance<CardDefinition>();

            var existing = LoadAllCards().FirstOrDefault();
            string folder = existing != null
                ? System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(existing))
                : "Assets";

            string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/New Card.asset");
            AssetDatabase.CreateAsset(card, path);
            AssetDatabase.SaveAssets();

            ShowCards();
            EditorGUIUtility.PingObject(card);
        }

        private void DeleteCard(CardDefinition card)
        {
            bool confirm = EditorUtility
                .DisplayDialog("Delete Card",$"Do you really want to delete this \"{card.name}\" data?","confirm","cancel");
            if (!confirm) return;
            string path = AssetDatabase.GetAssetPath(card);
            AssetDatabase.DeleteAsset(path);
            
            ShowCards();
        }
        private void CollapseAllCards()
        {
            detail.Query<Foldout>().ForEach(f => f.value = false);      
        }

        

        #endregion

    }
}
