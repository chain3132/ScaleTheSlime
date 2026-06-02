using System.Collections.Generic;
using Gameplay.NodeSelection.UI.Nodes.Enums;
using UnityEngine;

namespace Gameplay.NodeSelection.UI.Nodes
{
    [CreateAssetMenu(fileName = "NodeDatabase", menuName = "NodeSelection/NodeDatabase")]
    public class NodeDatabase : ScriptableObject
    {
        [SerializeField] private List<NodeDefinition> _definitions = new();
 
        private Dictionary<NodeType, NodeDefinition> _byType;
 
        public NodeDefinition Get(NodeType type)
        {
            if (_byType == null)
            {
                _byType = new Dictionary<NodeType, NodeDefinition>();
                foreach (var d in _definitions)
                    if (d != null) _byType[d.Type] = d;
            }
            return _byType.TryGetValue(type, out var def) ? def : null;
        }
 
        public Sprite SpriteFor(NodeType type)
        {
            var def = Get(type);
            return def != null ? def.Sprite : null;
        }
 
        
        public IReadOnlyList<NodeWeight> BuildWeightTable()
        {
            var table = new List<NodeWeight>();
            foreach (var d in _definitions)
            {
                if (d == null || d.Type == NodeType.Start || d.Type == NodeType.Boss)
                    continue;
                table.Add(new NodeWeight(d.Type, d.Weight, d.MinLayer));
            }
            return table;
        }
    }
}
