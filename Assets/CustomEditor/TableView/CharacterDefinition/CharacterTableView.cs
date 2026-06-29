using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Characters.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditor
{
    public class CharacterTableView : VisualElement
    {
        private SerializedObject serializedObject;
        private VisualElement _issuesBody;
        private HelpBox _banner;
        private enum Sev { Error, Warning }
        private readonly Dictionary<string, VisualElement> _cells = new();
        private readonly VisualElement _detail; 
        private readonly List<VisualElement> _rows = new();
        private string _searchText = "";
        private readonly Type _type;



        private struct Rule
        {
            public string Path, Location, Message; 
            public Sev Severity;
        }
        private static readonly Rule[] Rules =
        {
            new() { Path="TinySkeleton",   Location="Forms ▸ Tiny ▸ Skeleton",   Severity=Sev.Error,   Message="Skeleton not assigned" },
            new() { Path="NormalSkeleton", Location="Forms ▸ Normal ▸ Skeleton", Severity=Sev.Error,   Message="Skeleton not assigned" },
            new() { Path="GiantSkeleton",  Location="Forms ▸ Giant ▸ Skeleton",  Severity=Sev.Error,   Message="Skeleton not assigned" },
            new() { Path="TinyBehavior",   Location="Forms ▸ Tiny ▸ Behavior",   Severity=Sev.Error,   Message="Required behavior is not assigned" },
            new() { Path="NormalBehavior", Location="Forms ▸ Normal ▸ Behavior", Severity=Sev.Error,   Message="Required behavior is not assigned" },
            new() { Path="GiantBehavior",  Location="Forms ▸ Giant ▸ Behavior",  Severity=Sev.Error,   Message="Required behavior is not assigned" },
            new() { Path="TinyPassive",    Location="Forms ▸ Tiny ▸ Passive",    Severity=Sev.Warning, Message="Passive not assigned" },
            new() { Path="NormalPassive",  Location="Forms ▸ Normal ▸ Passive",  Severity=Sev.Warning, Message="Passive not assigned" },
            new() { Path="GiantPassive",   Location="Forms ▸ Giant ▸ Passive",   Severity=Sev.Warning, Message="Passive not assigned" },
            new() { Path="BaseMaterial",   Location="Outline ▸ BaseMaterial",   Severity=Sev.Warning, Message="BaseMaterial not assigned" },
            new() { Path="OutlineMaterial",Location="Outline ▸ Outline Material",Severity=Sev.Warning, Message="Not set — highlight outline will be disabled" },
        };
        
        public CharacterTableView(Type type)
        {
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/CustomEditor/TableView/CharacterDefinition/CharacterTableView.uss");
            _type = type;  
            styleSheets.Add(uss);
            style.flexGrow = 1;
            var split = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            split.style.flexGrow = 1;
            Add(split);
            
            _detail = new VisualElement { style = { flexGrow = 1 } };
            var detailScroll = new ScrollView { style = { flexGrow = 1 } };
            detailScroll.Add(_detail);
            split.Add(MakeSidebar());
            split.Add(detailScroll);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                var first = LoadAll().FirstOrDefault(e => e != null);
                if (first != null) ShowDetail(first);
            });

        }
        private List<CharacterDefinition> LoadAll()
        {
            return AssetDatabase.FindAssets($"t:{_type.Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<CharacterDefinition>)
                .Where(c => c != null).ToList();
        }

        private VisualElement MakeSidebar()
        {
            var nav = new VisualElement { style = { flexGrow = 1 } };
            nav.AddToClassList("nav");
            nav.style.flexGrow = 1;
        
            var scroll = new ScrollView{style = { flexGrow = 1 }};
            scroll.style.flexGrow = 1;
            scroll.Add(MakeHeader());

            (int order, string label) Key(CharacterDefinition character)
            {
                return character is EnemyDefinition enemy ? ((int)enemy.EnemyTier, enemy.EnemyTier.ToString()) : (0, "Player");
            }

            foreach (var group in LoadAll().GroupBy(Key).OrderBy(group => group.Key.order))
            {
                var foldout = new Foldout
                {
                    text = group.Key.ToString(),
                    value = true
                };
                foldout.AddToClassList("nav-foldout");
                foreach (var character in group.OrderBy(x => x.name))
                {
                    var label = new Label(character.name);
                    label.AddToClassList("nav-item");
                    label.RegisterCallback<ClickEvent>(_ => ShowDetail(character));
                    foldout.Add(label);
                }
                scroll.Add(foldout);
            }
            nav.Add(scroll);
            return nav;
        }
        private void ShowDetail(CharacterDefinition character)
        {
            _cells.Clear();
            serializedObject = new SerializedObject(character);
            _banner = new HelpBox("", HelpBoxMessageType.Warning) { style = { display = DisplayStyle.None } };

            var content = new VisualElement { style = { flexGrow = 1 } };
            content.Add(MakeHeaderDetail(character.name));
            content.Add(_banner);
            AddSection(content, "PROPERTIES",       MakeFields(serializedObject, "MaxHealth", "StartSize", "EnemyTier"));
            AddSection(content, "FORMS",            MakeFormsTable(serializedObject));
            AddSection(content, "HIGHLIGHT OUTLINE",MakeFields(serializedObject, "BaseMaterial", "OutlineMaterial"));
            AddSection(content, "SIZE DEATH RULES", MakeFields(serializedObject, "_diesWhenTooSmall", "_diesWhenTooBig"));
            AddSection(content, "STARTING / UNIQUE",MakeFields(serializedObject, "Unique2", "Unique5", "Unique8", "StartingDeck"));
            content.Add(MakeIssuesPanel());

            content.Bind(serializedObject);
            content.TrackSerializedObjectValue(serializedObject, Revalidate);

            _detail.Clear();
            _detail.Add(content);
            Revalidate(serializedObject);
        }
        private void AddSection(VisualElement parent, string title, VisualElement body)
        {
            if (body.childCount == 0) return;
            parent.Add(Section(title, body));
        }
        private VisualElement MakeHeader()
        {
            var header = new VisualElement();
            header.AddToClassList("header");
            var label = new Label("EnemyDefinition");
            int count = AssetDatabase.FindAssets($"t:EnemyDefinition").Length;
            var subLabel = new Label($"{count} assets ground by character");
            subLabel.AddToClassList("subLabel");
            header.Add(label);
            header.Add(subLabel);
            var search = new ToolbarSearchField();
            search.RegisterValueChangedCallback(e =>
            {
                _searchText = e.newValue;
                ApplyFilter();
            });
            header.Add(search);
            return header;

        }
        private VisualElement MakeHeaderDetail(string enemyName)
        {
            var bar = new VisualElement();
            bar.AddToClassList("enemy-header");

            var title = new Label(enemyName);                    
            title.AddToClassList("enemy-header-title");
            bar.Add(title);
            return bar;
        }
        private VisualElement MakeFields(SerializedObject serializedObject, params string[] names)
        {
            var v = new VisualElement();
            v.AddToClassList("section-Fields");
            foreach (var name in names)
                if (serializedObject.FindProperty(name) != null) v.Add(MakeBoundCell(serializedObject, name)); 
            return v;
        }
        private void ApplyFilter()
        {
            string words = (_searchText ?? "").ToLower();

            foreach (var foldout  in _rows)
            {
                var names  = foldout.userData as string[];
            
                bool matchSearch = words == "" || ((Foldout)foldout).text.ToLower().Contains(words) || (names != null && names.Any(n => n.Contains(words)));;
                foldout.style.display = (matchSearch) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
        private (string cha, string s) ParseName(string itemName)
        {
            var parts = itemName.Split('_');
            string character = parts.Length > 0 ? parts[0] : itemName;
            string size = parts.Length  > 1 ? parts[1] : "";
            return (character, size);
        }
        
        private PropertyField MakeBoundCell(SerializedObject serializedObject, string path, bool hideLabel = false)
        {
            var property = serializedObject.FindProperty(path);
            var cell = new PropertyField(property);
            if (hideLabel) cell.label = "";
            if (property != null) _cells[path] = cell;
            return cell;
        }

        private VisualElement Section(string title, VisualElement body)
        {
            var box = new VisualElement();
            box.AddToClassList("section");
            var header = new Label(title);
            header.AddToClassList("section-header");
            box.Add(header);
            box.Add(body);
            return box;
        }

        private void Revalidate(SerializedObject serializedObject)
        {
            serializedObject.Update();
            _issuesBody.Clear();
            int errors = 0, warns = 0;

            foreach (var rule in Rules)
            {
                var property = serializedObject.FindProperty(rule.Path);
                bool missing = property != null && property.propertyType == SerializedPropertyType.ObjectReference &&
                               property.objectReferenceValue == null;
                if (_cells.TryGetValue(rule.Path,out var cell))
                {
                    cell.EnableInClassList("cell-error",missing && rule.Severity == Sev.Error);
                    cell.EnableInClassList("cell-warning",missing && rule.Severity == Sev.Warning);
                }

                if (missing)
                {
                    if (rule.Severity == Sev.Error)
                    {
                        errors++;
                    }
                    else
                    {
                        warns++;
                    }
                    _issuesBody.Add(IssueRow(rule.Severity,rule.Location,rule.Message));
                }
            }

            int total = errors + warns;
            _banner.style.display = total == 0 ? DisplayStyle.None : DisplayStyle.Flex;
            if (total > 0)
            {
                _banner.messageType = errors > 0 ? HelpBoxMessageType.Error : HelpBoxMessageType.Warning;
                _banner.text = $"{errors} error - {warns} warnings in {serializedObject.targetObject.name}" + "— missing required fields will break at runtime";
            }
        }

        private VisualElement IssueRow(Sev severity, string location, string message)
        {
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
            row.AddToClassList("issue-row");

            var kind = new Label(severity == Sev.Error ? "Error" : "Warning") { style = { width = 60 } };
            row.AddToClassList(severity == Sev.Error ? "issue-error":"issue-warning");
            row.Add(kind);
            
            row.Add(new Label(location){style = {width = 160}});
            row.Add(new Label(message)  { style = { flexGrow = 1, whiteSpace = WhiteSpace.Normal } });
            row.Add(new Button(() => EditorGUIUtility.PingObject(serializedObject.targetObject)) { text = "Locate" });
            return row;
            
        }
        private VisualElement MakeFormsTable(SerializedObject serializedObject)
        {
            var rowSpecs = new (string label,string[] property)[]
            {
                ("Skeleton",      new[] { "TinySkeleton", "NormalSkeleton", "GiantSkeleton" }),
                ("Root Y Offset", new[] { "TinyOffsetY",  "NormalOffsetY",  "GiantOffsetY"  }),
                ("Behavior",      new[] { "TinyBehavior", "NormalBehavior", "GiantBehavior" }),
                ("Passive",       new[] { "TinyPassive",  "NormalPassive",  "GiantPassive"  }),
            };
            string[] columns = { "Tiny", "Normal", "Giant" };
            float labelWidth = 110, cellWidth = 230;
            var table = new VisualElement();

            var header = new VisualElement{ style = { flexDirection = FlexDirection.Row }};
            header.AddToClassList("section-FormsTable-Header");
            header.Add(new Label(""){style =
            {
                width = labelWidth,
                unityTextAlign = TextAnchor.MiddleLeft
            }});
            foreach (var column in columns)
                header.Add(MakeColumnHead(column, cellWidth));
            table.Add(header);
            foreach (var (label,properties) in rowSpecs)
            {
                var row = new VisualElement();
                row.AddToClassList("section-FormsTable");
                row.Add(new Label(label)
                {
                    style =
                    {
                        width = labelWidth,
                    }
                });
                foreach (var property in properties)
                {
                    var cell = MakeBoundCell(serializedObject, property, hideLabel: true);
                    cell.style.width = cellWidth;
                    row.Add(cell);
                }
                table.Add(row);
            }
            return table;
        }

        private static VisualElement MakeColumnHead(string column, float width)
        {
            var box = new VisualElement { style =
            {
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
                width = width
            }};
            box.Add(MakeCircleDot(DotSize(column)));
            box.Add(new Label(column));
            return box;
        }

        private static float DotSize(string column) => column.ToLowerInvariant() switch
        {
            "tiny"   => 8f,
            "normal" => 12f,
            "giant"  => 16f,
            _        => 10f,
        };

        private static VisualElement MakeCircleDot(float size)
        {
            var dot = new VisualElement();
            dot.style.width = size;
            dot.style.height = size;
            dot.style.marginRight = 5;
            dot.style.marginLeft = 10;
            dot.style.flexShrink = 0;
            dot.style.backgroundColor = new StyleColor(new Color(0.36f, 0.61f, 0.88f));

            var radius = new StyleLength(new Length(50, LengthUnit.Percent));
            dot.style.borderTopLeftRadius = radius;
            dot.style.borderTopRightRadius = radius;
            dot.style.borderBottomLeftRadius = radius;
            dot.style.borderBottomRightRadius = radius;
            return dot;
        }

        private VisualElement MakeIssuesPanel()
        {
            var foldout = new Foldout { text = "Issues", value = true };
            foldout.AddToClassList("issues");
            _issuesBody = new VisualElement();
            foldout.Add(_issuesBody);
            return foldout;

        }
    }
}
