using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gameplay.NodeSelection.Encounter;
using Gameplay.NodeSelection.UI;
using Gameplay.NodeSelection.UI.DefeatPanel;
using Gameplay.NodeSelection.UI.Map;
using Gameplay.NodeSelection.UI.Map.MapGenerator;
using Gameplay.NodeSelection.UI.Nodes;
using Gameplay.NodeSelection.UI.Nodes.Enums;
using R3;
using UnityEngine;

namespace Gameplay.NodeSelection.UI 
{
    public class GameFlowController : IDisposable
    {
        
        private readonly MapView _mapView;
        private readonly RunState _runState;
        private readonly IEncounter _encounter;
        private readonly MapGenerationConfig _config;
        private readonly NodeDatabase _database;
        private readonly int _seed;
        private readonly bool _walkOnly;
        private MapViewModel _mapVm;
        private readonly CompositeDisposable _mapScope = new();
        private readonly DefeatPanelView _defeatPanel;
        
        public GameFlowController(MapView mapView,
            RunState runState, 
            IEncounter encounter,
            MapGenerationConfig config,
            NodeDatabase database,
            int seed,
            bool walkOnly,DefeatPanelView defeatPanel)
        {
            _mapView = mapView;
            _runState = runState;
            _encounter = encounter;
            _config = config;
            _database = database;
            _seed = seed;
            _walkOnly = walkOnly;
            _defeatPanel = defeatPanel;
        }
        
        public void BuildRunMap()
        {
            var generator = new MapGenerator(_config, _seed + _runState.RunNumber.Value,
                _database.BuildWeightTable());
            MapData map = generator.Generate();

            _mapVm = new MapViewModel(map, _runState);
            _mapView.Initialize(_mapVm, _database);
            _mapView.Show();

            _mapVm.NodeSelected
                .SubscribeAwait(OnNodeSelectedAsync, AwaitOperation.Drop)  
                .AddTo(_mapScope);
        }
        private async ValueTask OnNodeSelectedAsync(EncounterRequest req, CancellationToken ct)
        {
            await _mapView.MoveTokenToAsync(req.NodeId, ct);   

            if (_walkOnly)        
            {
                OnNodeCleared(req);
                return;
            }
            _mapView.Hide();
            var result = await _encounter.EnterAsync(req, ct);
            if (result.Outcome == EncounterOutcome.Defeat)
            {
                _defeatPanel.gameObject.SetActive(true);
                return;
            }  
            _mapView.Show();
            OnNodeCleared(req);
        }
        
        private void OnNodeCleared(EncounterRequest req)
        {
            _runState.MoveTo(req.NodeId);
            if (req.Type == NodeType.Boss)
            {
                NextRun().Forget();                     
            }
        }
        private async UniTaskVoid NextRun()
        {
            await UniTask.Yield();        
            _mapView.Teardown();
            _mapVm.Dispose();
            _mapScope.Clear();
            _runState.NextRun();
            BuildRunMap();
        }

        public void Dispose()
        {
            _mapVm?.Dispose();
            _mapScope.Dispose();
        }
    }
}
