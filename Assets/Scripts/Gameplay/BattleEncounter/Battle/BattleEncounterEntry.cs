using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.UI.Reward;
using Gameplay.NodeSelection.Encounter;
using Gameplay.NodeSelection.UI;
using UnityEngine;

namespace Gameplay.BattleEncounter.Battle
{
    public class BattleEncounterEntry : MonoBehaviour, IEncounter
    {
        [SerializeField]
        private BattleController _battleController;
        [SerializeField]
        private EncounterTable _encounterTable;
        [SerializeField]
        private GameObject _battleRoot;     
        [SerializeField]
        private RewardPanelView _rewardPanel;
        [SerializeField]
        private CardRewardPool _rewardPool;
        [SerializeField]
        private int _rewardChoices = 3;
        [SerializeField]
        private int _seed = 6789;
        [SerializeField]
        private bool _skipReward = false;

        private RunProgress _progress;

        public void Initialize(RunProgress progress) => _progress = progress;

        public async UniTask<EncounterResult> EnterAsync(EncounterRequest req, CancellationToken ct)
        {
            var rng = new System.Random(_seed + req.RunNumber * 1000 + req.NodeId);
            var enemies = _encounterTable != null
                ? _encounterTable.Roll(req.Type, req.Layer, rng)
                : null;   

            if (enemies != null && enemies.Count == 0)
                return EncounterResult.Victory;

            if (_battleRoot != null) _battleRoot.SetActive(true);
            try
            {
                bool win = await _battleController.RunAsync(_progress, enemies, ct);
                
                if (_skipReward) return win ? EncounterResult.Victory : EncounterResult.Defeat;
                if (win && _rewardPanel != null && _rewardPool != null)
                {
                    var choices = _rewardPool.Roll(_rewardChoices, rng);
                    var picked = await _rewardPanel.ShowAsync(choices, ct);
                    if (picked != null) _progress.Deck.Add(picked);   
                    else _progress.HealFull();                        
                }

                return win ? EncounterResult.Victory : EncounterResult.Defeat;
            }
            finally
            {
                if (_battleRoot != null) _battleRoot.SetActive(false);
            }
        }
    }
}
