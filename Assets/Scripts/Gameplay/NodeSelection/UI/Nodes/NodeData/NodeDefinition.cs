using Gameplay.NodeSelection.UI.Nodes.Enums;
using UnityEngine;

namespace Gameplay.NodeSelection.UI.Nodes
{
    [CreateAssetMenu(fileName = "NodeDefinition", menuName = "NodeSelection/NodeDefinition")]
    public class NodeDefinition : ScriptableObject
    {
        public NodeType Type;
        public Sprite Sprite;
 
        [Header("Random spawn — ignored for Start and Boss")]
        [Min(0)] 
        public int Weight = 10;  
        [Min(0)] 
        public int MinLayer = 0;
    }
}
