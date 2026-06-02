using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.NodeSelection.UI.Map;
using Gameplay.NodeSelection.UI.Map.MapGenerator;
using Gameplay.NodeSelection.UI.Nodes;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Gameplay.NodeSelection.UI
{
    public class NodeSelectionEntryPoint : MonoBehaviour
    {
        [Header("Scene Views")]
        [SerializeField] private MapView _mapView;
 
        [Header("Config")]
        [SerializeField] private MapGenerationConfig _mapConfig = new();
        [SerializeField] private NodeDatabase _nodeDatabase;
        [SerializeField] private string _playerName = "Slime Queen";
        [SerializeField] private string _nodePrefabAddress = "NodeView"; // Addressables key
        [SerializeField] private int _seed = 12345;
 
        private RunState _runState;
        private AsyncOperationHandle<GameObject> _nodePrefabHandle;
        private GameObject _nodePrefab;
        private CancellationTokenSource _cts;
 
        private MapViewModel _mapVm;
        private readonly CompositeDisposable _runScope = new();
 
        private async UniTaskVoid Start()
        {
            _cts = new CancellationTokenSource();
 
            _runState = new RunState();
 
            _nodePrefabHandle = Addressables.LoadAssetAsync<GameObject>(_nodePrefabAddress);
            _nodePrefab = await _nodePrefabHandle.ToUniTask(cancellationToken: _cts.Token);
            
            // Build First map.
            Debug.Log("Building first map...");
            BuildRunMap();
        }
 
        private void BuildRunMap()
        {
            var generator = new MapGenerator(_mapConfig, _seed + _runState.RunNumber.Value,_nodeDatabase.BuildWeightTable());
            MapData map = generator.Generate();
 
            _mapVm = new MapViewModel(map, _runState);
            _mapView.Initialize(_mapVm, _nodePrefab,_nodeDatabase);
 
            // When the boss is cleared, rebuild the map.
            _mapVm.BossCleared
                .Subscribe(_ => OnBossCleared())
                .AddTo(_runScope);
        }
 
        private void OnBossCleared()
        {
            _mapView.Teardown();
            _mapVm.Dispose();
            _runScope.Clear();
 
            _runState.NextRun();
            BuildRunMap();
        }
 
        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
 
            _mapView?.Teardown();
            _mapVm?.Dispose();
            _runScope.Dispose();
 
            //_panelMenuVm?.Dispose();
            _runState?.Dispose();
 
            if (_nodePrefabHandle.IsValid())
                Addressables.Release(_nodePrefabHandle);
        }
    }
}
