using System;
using Gameplay.NodeSelection.UI.Nodes.Enums;
using R3;
using UnityEngine;

namespace Gameplay.NodeSelection.UI.Nodes
{
    public class NodeViewModel : IDisposable
    {
        #region Properties

        public int Id { get; }
        public NodeType Type { get; }
        public int Layer { get; }
        public int IndexInLayer { get; }
 
        public ReadOnlyReactiveProperty<bool> IsCurrent { get; }
        public ReadOnlyReactiveProperty<bool> IsReachable { get; }
        public ReadOnlyReactiveProperty<bool> IsVisited { get; }

        #endregion

        #region fields

        private readonly Subject<int> _selectRequested;

        #endregion
        
        #region Constructors
        
        public NodeViewModel (Node node,
            ReadOnlyReactiveProperty<bool> isCurrent,
            ReadOnlyReactiveProperty<bool> isReachable,
            ReadOnlyReactiveProperty<bool> isVisited,
            Subject<int> selectRequested)
        {
            Id = node.Id;
            Type = node.Type;
            Layer = node.Layer;
            IndexInLayer = node.IndexInLayer;
            IsCurrent = isCurrent;
            IsReachable = isReachable;
            IsVisited = isVisited;
            _selectRequested = selectRequested;
        }
        #endregion

        #region Methods

        public void Select()    
        {
            _selectRequested.OnNext(Id);
        }
        
        public void Dispose()
        {
            IsCurrent.Dispose();
            IsReachable.Dispose();
            IsVisited.Dispose();
        }

        #endregion
        
    }
}
