using System.Collections.Generic;
using System.Threading;
using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.NodeSelection.Encounter;
using Gameplay.NodeSelection.UI.DefeatPanel;
using Gameplay.NodeSelection.UI.Map;
using Gameplay.NodeSelection.UI.Map.MapGenerator;
using Gameplay.NodeSelection.UI.Nodes;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.NodeSelection.UI
{
    public class NodeSelectionEntryPoint : MonoBehaviour
    {
        [Header("Scene Views")]
        [SerializeField]
        private MapView _mapView;
        [FormerlySerializedAs("_panelMenuView")] [SerializeField]
        private MenuPanelView menuPanelView;
        [SerializeField]
        private BattleEncounterEntry _battleEncounter;   
        [SerializeField]
        private PlayerDefinition _playerDefinition;
        [SerializeField] 
        private DefeatPanelView _defeatPanel;

        [Header("Config")]
        [SerializeField] 
        private MapGenerationConfig _mapConfig = new();
        [SerializeField] 
        private NodeDatabase _nodeDatabase;
        [SerializeField]
        private string _playerName = "Slime Queen";
        [SerializeField]
        private int _seed = 12345;
        
        [Header("Debug")]
        [SerializeField] 
        private bool _walkOnly = true;     
        [SerializeField] 
        private bool _forceDefeat = false;
 
        private RunState _runState;
        private RunProgress _runProgress;
        private MenuPanelViewModel _menuPanelVm;
        private CancellationTokenSource _cts;
        private GameFlowController _flow;
        private readonly CompositeDisposable _runScope = new();
 
        private void Start()
        {
            _cts = new CancellationTokenSource();

            _runState = new RunState();

            _menuPanelVm = new MenuPanelViewModel(_playerName, _runState);
            menuPanelView.Bind(_menuPanelVm);
            
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
                _mapConfig, _nodeDatabase, _seed, _walkOnly,_defeatPanel);
            _flow.BuildRunMap();
        }
        
 
        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
 
            _mapView?.Teardown();
            _menuPanelVm?.Dispose();
            _runScope.Dispose();
            _flow?.Dispose();
            _runState?.Dispose();
        }
    }
}
