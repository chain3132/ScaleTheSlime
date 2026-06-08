using System.Collections.Generic;
using Gameplay.NodeSelection.UI.Nodes.Enums;
using UnityEngine;

namespace Gameplay.NodeSelection.UI.Nodes
{
    public class Node 
    {
        #region Properties
        
        public int Id { get; }
        public int Layer { get; }          
        public int IndexInLayer { get; }   
        public NodeType Type { get; set; } 
        public List<int> NextIds { get; } = new(); 
        
        #endregion

        #region Constructors

        public Node(int id, 
            int layer, 
            int indexInLayer)
        {
            Id = id;
            Layer = layer;
            IndexInLayer = indexInLayer;
        }

        #endregion
        
    }
}
