using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Card.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditor.TableView
{
    public class CardTableView : VisualElement
    {
        private readonly VisualElement _rowsContainer = new();
        private string _sortMode = "name";
        private string _searchText = "";
        private CardEffectType? _filterType = null;
        private readonly List<VisualElement> _rows = new();
    
    
        public CardTableView()
        {
            style.flexGrow = 1;

            Add(MakeToolbar());          
            Add(MakeHeader());          

            var scroll = new ScrollView();
            scroll.style.flexGrow = 1;
            scroll.Add(_rowsContainer);
            Add(scroll);

            ShowCards();
        }
        #region Cards

        private VisualElement MakeHeader()
        {
            var h = new VisualElement();
            h.AddToClassList("Header");
            foreach (var (name, w) in new[] {("",20),("Preview",70),("DisplayName",120),("Description",300),("Art",300),("Effects",280)})
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
            sortMenu.menu.AppendAction("name a-z", _ => SetSort("name"));
            bar.Add(sortMenu);

            var create = new ToolbarButton(OpenCreateWizard) { text = "+ New" };
            create.AddToClassList("create-button");
            var refresh = new ToolbarButton(ShowCards) { text = "Refresh" };
            bar.Add(create);
            bar.Add(refresh);
            bar.Add(new ToolbarButton(CollapseAllCards) {text = "Collapse All"});
            var search = new ToolbarSearchField();
            search.RegisterValueChangedCallback(e =>
            {
                _searchText = e.newValue;
                ApplyFilter();
            });
            bar.Add(search);
            bar.Add(MakeFilter());
            
            return bar;
        }

        private void OpenCreateWizard()
        {
            CardCreationWindow.Open(CreateNewCard);
        }

        private ToolbarMenu MakeFilter()
        {
            var filterMenu = new ToolbarMenu { text = "Effect: All" };
            filterMenu.menu.AppendAction("All", _ =>
            {
                _filterType = null;
                filterMenu.text = "Effect: All";
                ApplyFilter();
            });
            foreach (CardEffectType t in Enum.GetValues(typeof(CardEffectType)))
            {
                var effectType = t;
                filterMenu.menu.AppendAction(t.ToString(), _ =>
                {
                    _filterType = effectType;
                    filterMenu.text = $"Effect: {effectType}";
                    ApplyFilter();
                });
            }

            return filterMenu;
        }
        private VisualElement MakeRow(CardDefinition card ,int index)
        {
            var so = new SerializedObject(card);
            
            var row = new VisualElement();
            row.AddToClassList("row");
            if (index % 2 == 1)
                row.AddToClassList("row-odd");
            row.userData = card;
            var deleteButton = new Button((() => DeleteCard(card))) { text = "X" };
            deleteButton.AddToClassList("delete-button");
            row.Add(deleteButton);
            
            row.Add(MakePreview(card));
            AddCell(row, so, "DisplayName", 120);
            AddCell(row,so ,"Description",300);
            AddCell(row, so, "Art",         300);
            AddCell(row, so, "Effects",     280);
            
            
            row.Bind(so);       
            _rows.Add(row); 
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

        private void ApplyFilter()
        {
            string words = (_searchText ?? "").ToLower();

            foreach (var row in _rows)
            {
                var card = row.userData as CardDefinition;
                if (card == null) continue;

                string name = ((card.DisplayName ?? "") + " " + card.name).ToLower();

                bool matchSearch = words == "" || name.Contains(words);

                bool matchType = _filterType == null
                                 || (card.Effects != null && card.Effects.Any(e => e.Type == _filterType));

                row.style.display = (matchSearch && matchType) ? DisplayStyle.Flex : DisplayStyle.None;
            }
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
                // case "id_asc":  cards = cards.OrderBy(c => c.Id); break;
                // case "id_desc": cards = cards.OrderByDescending(c => c.Id); break;
                default:        cards = cards.OrderBy(c => c.name); break;
            }
            return cards.ToList();
        }
        private void ShowCards()
        {
            _rowsContainer.Clear();      
            _rows.Clear();

            var cards = LoadAllCards();
            for (int i = 0; i < cards.Count; i++)
                _rowsContainer.Add(MakeRow(cards[i], i));
        }
        private void CreateNewCard(CardCreationRequest eRequest)
        {
            var card = eRequest.Preset;
            

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
            _rowsContainer.Query<Foldout>().ForEach(f => f.value = false);
        }

        private VisualElement MakePreview(CardDefinition card)
        {
            var art = card.Art;
            var boxPreview = new VisualElement();
            boxPreview.AddToClassList("box-card-preview");
            if (art != null)
            {
                AddImageCardPreview(boxPreview,art.CardArt,14, 14, 6, 30, ScaleMode.ScaleAndCrop);
                AddImageCardPreview(boxPreview,art.CardBackground, 0, 0, 0, 0, ScaleMode.ScaleToFit);
                AddImageCardPreview(boxPreview,art.CardHeader, 10, 10, -85, 0, ScaleMode.ScaleToFit);
                AddTextCardPreview(boxPreview,card.DisplayName,10,10,-85,0);

            }
            return boxPreview;
        }

        private void AddImageCardPreview(VisualElement parent,Sprite sprite , float left, float right, float top, float bottom, ScaleMode mode )
        {
            if (sprite == null) return; 
            var img = new Image { sprite = sprite, scaleMode = mode  };
            img.AddToClassList("img-card");
            img.style.left   = Length.Percent(left);
            img.style.right  = Length.Percent(right);
            img.style.top    = Length.Percent(top);
            img.style.bottom = Length.Percent(bottom);
            parent.Add(img);
            
        }

        private void AddTextCardPreview(VisualElement parent, string text, float left, float right, float top, float bottom)
        {
            var label = new Label(text);
            label.AddToClassList("preview-text");
            label.style.left = Length.Percent(left);;
            label.style.right = Length.Percent(right);
            label.style.top = Length.Percent(top);    
            label.style.bottom = Length.Percent(bottom);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            parent.Add(label);
        }

        

        #endregion
    }
}
