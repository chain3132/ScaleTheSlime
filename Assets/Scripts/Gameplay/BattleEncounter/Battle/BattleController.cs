using System.Collections.Generic;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Characters;
using R3;
using UnityEngine;

namespace Gameplay.BattleEncounter.Battle
{
    
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private PlayerDefinition _playerDefinition;
        [SerializeField] private CharacterView _playerView;
        [SerializeField] private CardPlayController _playController;
        [SerializeField] private List<EnemyView> _enemyViews = new();

        private Player _player;
        private readonly List<Enemy> _enemies = new();
        private BattleContext _context;
        private CardEffectExecutor _executor;
        private readonly CompositeDisposable _scope = new();

        private void Start()
        {
            _player = new Player(_playerDefinition);
            if (_playerView != null) _playerView.Bind(_player);

            foreach (var ev in _enemyViews)
                if (ev != null) _enemies.Add(ev.Create());

            _context = new BattleContext(_player, _enemies, null);
            _executor = new CardEffectExecutor(_context);
            _player.Activate(_context);

            _playController.CardPlayed
                .Subscribe(play => _executor.Execute(play.Card, play.Target))
                .AddTo(_scope);

        }

        private void OnDestroy()
        {
            _scope.Dispose();
            _player?.Dispose();
            foreach (var e in _enemies) e?.Dispose();
        }
    }
}
