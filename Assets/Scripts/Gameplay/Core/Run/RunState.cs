using System;
using System.Collections.Generic;
using R3;

namespace Gameplay.NodeSelection.UI
{
    public class RunState : IDisposable
    {
        #region Properties
        public ReactiveProperty<int> RunNumber { get; } = new(1);
        public ReactiveProperty<int> CurrentNodeId { get; } = new(-1);
        public ReactiveProperty<int> BattleTurn { get; } = new(0);   

        #endregion

        #region fields

        private readonly HashSet<int> _visited = new();
        
        #endregion
 
        public bool IsVisited(int nodeId) => _visited.Contains(nodeId);
 
        public void MoveTo(int nodeId)
        {
            if (CurrentNodeId.Value >= 0) _visited.Add(CurrentNodeId.Value);
            _visited.Add(nodeId);
            CurrentNodeId.Value = nodeId; 
        }
 
        public void NextRun()
        {
            _visited.Clear();
            CurrentNodeId.Value = -1;
            RunNumber.Value += 1;
        }
        public void Dispose()
        {
            RunNumber.Dispose();
            CurrentNodeId.Dispose();
            BattleTurn.Dispose();
        }
    }
}



