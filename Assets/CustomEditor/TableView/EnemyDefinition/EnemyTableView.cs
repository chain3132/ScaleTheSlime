using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditor
{
    public class EnemyTableView : VisualElement
    {
        private SerializedObject _so;
        private VisualElement _issuesBody;
        private HelpBox _banner;
        private enum Sev { Error, Warning }
        private readonly Dictionary<string, VisualElement> _cells = new();

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
        
        public EnemyTableView(Gameplay.BattleEncounter.Characters.Data.EnemyDefinition enemyDefinition)
        {
            var serializedObject = new SerializedObject(enemyDefinition);
            _so = serializedObject;
            style.flexGrow = 1;
            _banner = new HelpBox("", HelpBoxMessageType.Warning)
            {
                style = { display = DisplayStyle.None }
            };
            Add(_banner);
            Add(Section("PROPERTIES", MakeFields(serializedObject,"MaxHealth", "StartSize")));
            Add(Section("FORMS",MakeFormsTable(serializedObject)));
            Add(Section("HIGHLIGHT OUTLINE", MakeFields(serializedObject, "BaseMaterial", "OutlineMaterial")));
            Add(Section("SIZE DEATH RULES",  MakeFields(serializedObject, "_diesWhenTooSmall", "_diesWhenTooBig")));
            Add(MakeIssuesPanel());
            this.Bind(serializedObject);
            this.TrackSerializedObjectValue(serializedObject,Revalidate);
            
            Revalidate(serializedObject);
        }

        private VisualElement MakeFields(SerializedObject serializedObject, params string[] names)
        {
            var visualElement = new VisualElement();
            foreach (var name in names) visualElement.Add(MakeBoundCell(serializedObject, name));
            visualElement.AddToClassList("section-Fields");
            return visualElement;
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
            row.Add(new Button(() => EditorGUIUtility.PingObject(_so.targetObject)) { text = "Locate" });
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
