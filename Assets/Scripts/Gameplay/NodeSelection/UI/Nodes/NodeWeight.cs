using Gameplay.NodeSelection.UI.Nodes.Enums;

namespace Gameplay.NodeSelection.UI.Nodes
{
    public readonly struct NodeWeight
    {
    
        public readonly NodeType Type;
        public readonly int Weight;    
        public readonly int MinLayer;  
 
        public NodeWeight(NodeType type, int weight, int minLayer)
        {
            Type = type;
            Weight = weight;
            MinLayer = minLayer;
        }
    }
}
