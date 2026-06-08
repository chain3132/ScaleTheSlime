using System;
using System.Collections.Generic;
using Gameplay.NodeSelection.Encounter;
using Gameplay.NodeSelection.UI.Nodes;
using R3;

namespace Gameplay.NodeSelection.UI.Map
{
    public readonly struct MapEdge
    {
        public readonly int FromId;
        public readonly int ToId;
        public MapEdge(int fromId, int toId) { FromId = fromId; ToId = toId; }
    }
    
    public class MapViewModel : IDisposable
    {
        public IReadOnlyList<NodeViewModel> Nodes => _nodes;
        public IReadOnlyList<MapEdge> Edges => _edges;
 
        public Observable<EncounterRequest> NodeSelected => _nodeSelected;
            
        private readonly Subject<EncounterRequest> _nodeSelected = new();
        private readonly RunState _runState;
        private readonly MapData _map;
        private readonly List<NodeViewModel> _nodes = new();
        private readonly List<MapEdge> _edges = new();
 
        private readonly Subject<int> _selectRequested = new();
        
        private readonly CompositeDisposable _disposables = new();
        
        public MapViewModel(MapData mapData, 
            RunState runState)
        {
            _map = mapData;
            _runState = runState;
 
            BuildNodeViewModels();
            BuildEdges();
 
            _selectRequested
                .Subscribe(OnSelect)
                .AddTo(_disposables);
 
            // Put the player on the start node at the beginning of a run
            if (_runState.CurrentNodeId.Value < 0)
                _runState.MoveTo(_map.StartId);

        }
         private void BuildNodeViewModels()
        {
            foreach (var node in _map.Nodes)
            {
                int id = node.Id;
                
                var isCurrent = _runState.CurrentNodeId
                    .Select(cur => cur == id)
                    .ToReadOnlyReactiveProperty();
 
                var isReachable = _runState.CurrentNodeId
                    .Select(cur => cur >= 0 && _map.Get(cur).NextIds.Contains(id))
                    .ToReadOnlyReactiveProperty();
 
                var isVisited = _runState.CurrentNodeId
                    .Select(_ => _runState.IsVisited(id))
                    .ToReadOnlyReactiveProperty();
 
                _nodes.Add(new NodeViewModel(node, isCurrent, isReachable, isVisited, _selectRequested));
            }
        }
 
        private void BuildEdges()
        {
            foreach (var node in _map.Nodes)
                foreach (var nextId in node.NextIds)
                    _edges.Add(new MapEdge(node.Id, nextId));
        }
 
        private void OnSelect(int nodeId)
        {
            int current = _runState.CurrentNodeId.Value;
            bool reachable = current >= 0 && _map.Get(current).NextIds.Contains(nodeId);
            if (!reachable) return; // ignore click on nodes that cannot reach
            
            //enter the node
            var node = _map.Get(nodeId);
            _nodeSelected.OnNext(new EncounterRequest(
                nodeId, node.Type, node.Layer, _runState.RunNumber.Value));
        }
 
        public void Dispose()
        {
            foreach (var n in _nodes) n.Dispose();
            _nodes.Clear();
            _nodeSelected.Dispose();    
            _selectRequested.Dispose();
            _disposables.Dispose();
        }
    }
}
