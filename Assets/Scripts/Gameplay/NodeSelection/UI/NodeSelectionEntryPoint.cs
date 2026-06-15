using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.NodeSelection.Encounter;
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
        [SerializeField]
        private MapView _mapView;
        [SerializeField]
        private PanelMenuView _panelMenuView;
        [SerializeField]
        private BattleEncounterEntry _battleEncounter;   
        [SerializeField]
        private PlayerDefinition _playerDefinition;       

        [Header("Config")]
        [SerializeField] 
        private MapGenerationConfig _mapConfig = new();
        [SerializeField] 
        private NodeDatabase _nodeDatabase;
        [SerializeField] 
        private string _playerName = "Slime Queen";
        [SerializeField] 
        private string _nodePrefabAddress = "NodeView";
        [SerializeField] 
        private int _seed = 12345;
        
        [Header("Debug")]
        [SerializeField] 
        private bool _walkOnly = true;     
        [SerializeField] 
        private bool _forceDefeat = false;
 
        private RunState _runState;
        private RunProgress _runProgress;
        private PanelMenuViewModel _panelMenuVm;
        private AsyncOperationHandle<GameObject> _nodePrefabHandle;
        private GameObject _nodePrefab;
        private CancellationTokenSource _cts;
        private GameFlowController _flow;
        private readonly CompositeDisposable _runScope = new();
 
        private async UniTaskVoid Start()
        {
            _cts = new CancellationTokenSource();
 
            _runState = new RunState();
            
            _nodePrefabHandle = Addressables.LoadAssetAsync<GameObject>(_nodePrefabAddress);
            _nodePrefab = await _nodePrefabHandle.ToUniTask(cancellationToken: _cts.Token);
            
            _panelMenuVm = new PanelMenuViewModel(_playerName, _runState);
            _panelMenuView.Bind(_panelMenuVm);
            
            IEncounter encounter;
            if (_battleEncounter != null && _playerDefinition != null)
            {
                var startingCards = _playerDefinition.StartingDeck != null
                    ? new List<CardDefinition>(_playerDefinition.StartingDeck.Cards)
                    : new List<CardDefinition>();
                _runProgress = new RunProgress(_playerDefinition.MaxHealth, startingCards);
                _battleEncounter.Initialize(_runProgress);
                encounter = _battleEncounter;
            }
            else
            {
                encounter = new EncounterEntry(_forceDefeat);   
            }

            _flow = new GameFlowController(_mapView, _runState, encounter,
                _mapConfig, _nodeDatabase, _nodePrefab, _seed, _walkOnly);
            _flow.BuildRunMap();
        }
        
 
        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
 
            _mapView?.Teardown();
            _panelMenuVm?.Dispose();
            _runScope.Dispose();
            _flow?.Dispose();
            _runState?.Dispose();
 
            if (_nodePrefabHandle.IsValid())
                Addressables.Release(_nodePrefabHandle);
        }
    }
}
