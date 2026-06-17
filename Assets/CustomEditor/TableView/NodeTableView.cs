using System.Collections.Generic;
using System.Linq;
using Gameplay.NodeSelection.UI.Nodes;
using Gameplay.NodeSelection.UI.Nodes.Enums;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditor.TableView
{
    public class NodeTableView : VisualElement
    {
        private readonly VisualElement _rowsContainer = new();
        private const float PreviewSize = 64f;

        public NodeTableView()
        {
            style.flexGrow = 1;

            Add(MakeToolbar());
            Add(MakeHeader());

            var scroll = new ScrollView();
            scroll.style.flexGrow = 1;
            scroll.Add(_rowsContainer);
            Add(scroll);

            ShowNodes();
        }

        #region Toolbar / Header

        private VisualElement MakeToolbar()
        {
            var bar = new Toolbar();

            var create = new ToolbarButton(CreateNewNode) { text = "+ New" };
            create.AddToClassList("create-button");
            bar.Add(create);

            bar.Add(new ToolbarButton(ShowNodes) { text = "Refresh" });

            return bar;
        }

        private VisualElement MakeHeader()
        {
            var h = new VisualElement();
            h.AddToClassList("Header");
            foreach (var (name, w) in new[]
                     { ("Preview", 70), ("Type", 110), ("Prefab", 220), ("Weight", 90), ("MinLayer", 90) })
            {
                var lbl = new Label(name);
                lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
                lbl.style.width = w;
                lbl.style.height = 20;
                h.Add(lbl);
            }
            return h;
        }

        #endregion

        #region Rows

        private VisualElement MakeRow(NodeDefinition def, int index, bool duplicateType)
        {
            var so = new SerializedObject(def);

            var row = new VisualElement();
            row.AddToClassList("row");
            if (index % 2 == 1)
                row.AddToClassList("row-odd");
            row.userData = def;

            row.Add(MakePreview(def));

            AddCell(row, so, "Type", 110);
            AddCell(row, so, "Prefab", 220);

            bool randomIgnored = def.Type == NodeType.Start || def.Type == NodeType.Boss;
            var weightCell = AddIntCell(row, "Weight", 90);
            var minLayerCell = AddIntCell(row, "MinLayer", 90);
            if (randomIgnored)
            {
                weightCell.SetEnabled(false);
                minLayerCell.SetEnabled(false);
            }

            if (duplicateType)
            {
                var warn = new Label("⚠") { tooltip = $"Duplicate Type '{def.Type}'. " +
                    "NodeDatabase keeps only one definition per Type." };
                warn.style.color = new Color(1f, 0.7f, 0.2f);
                warn.style.unityTextAlign = TextAnchor.MiddleCenter;
                warn.style.width = 22;
                row.Add(warn);
            }

            var deleteButton = new Button(() => DeleteNode(def)) { text = "X" };
            deleteButton.AddToClassList("delete-button");
            row.Add(deleteButton);

            row.Bind(so);
            return row;
        }

        private PropertyField AddCell(VisualElement row, SerializedObject so, string prop, float width)
        {
            var field = new PropertyField(so.FindProperty(prop));
            field.label = "";
            field.AddToClassList("cell");
            field.style.width = width;
            row.Add(field);
            return field;
        }

        
        private IntegerField AddIntCell(VisualElement row, string prop, float width)
        {
            var field = new IntegerField { bindingPath = prop };
            field.AddToClassList("cell");
            field.style.width = width;
            row.Add(field);
            return field;
        }


        private VisualElement MakePreview(NodeDefinition def)
        {
            var box = new VisualElement();
            box.AddToClassList("box-card-preview");
            box.style.width = PreviewSize;
            box.style.height = PreviewSize;

            if (def.Prefab == null) return box;

            var images = def.Prefab.GetComponentsInChildren<UnityEngine.UI.Image>(false)
                .Where(i => i.sprite != null)
                .ToList();
            if (images.Count == 0) return box;
            
            float largestImageSize = images.Max(i =>
                Mathf.Max(i.rectTransform.sizeDelta.x, i.rectTransform.sizeDelta.y));


            foreach (var src in images)
            {
                var size = src.rectTransform.sizeDelta / largestImageSize * PreviewSize;

                var layer = new Image
                {
                    sprite = src.sprite,
                    tintColor = src.color,
                    scaleMode = ScaleMode.ScaleToFit,
                };
                layer.style.position = Position.Absolute;
                layer.style.width = size.x;
                layer.style.height = size.y;
                layer.style.left = (PreviewSize - size.x) * 0.5f;
                float top = (PreviewSize - size.y) * 0.5f;
                if (src.gameObject.name == "NodeTypeIcon") top -= 8f;
                layer.style.top = top;

                box.Add(layer);
            }

            return box;
        }

        #endregion

        #region Data

        private List<NodeDefinition> LoadAllNodes()
        {
            return AssetDatabase.FindAssets("t:NodeDefinition")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<NodeDefinition>)
                .Where(a => a != null)
                .OrderBy(d => (int)d.Type)
                .ToList();
        }

        private void ShowNodes()
        {
            _rowsContainer.Clear();

            var nodes = LoadAllNodes();

            var typeCount = nodes
                .GroupBy(d => d.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            for (int i = 0; i < nodes.Count; i++)
            {
                bool duplicate = typeCount[nodes[i].Type] > 1;
                _rowsContainer.Add(MakeRow(nodes[i], i, duplicate));
            }
        }

        private void CreateNewNode()
        {
            var def = ScriptableObject.CreateInstance<NodeDefinition>();

            var existing = LoadAllNodes().FirstOrDefault();
            string folder = existing != null
                ? System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(existing))
                : "Assets";

            string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/New NodeDefinition.asset");
            AssetDatabase.CreateAsset(def, path);
            AssetDatabase.SaveAssets();

            ShowNodes();
            EditorGUIUtility.PingObject(def);
        }

        private void DeleteNode(NodeDefinition def)
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Delete NodeDefinition",
                $"Do you really want to delete \"{def.name}\"?",
                "confirm", "cancel");
            if (!confirm) return;

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(def));
            ShowNodes();
        }

        #endregion
    }
}
