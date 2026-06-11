using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Characters;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BattleEncounter.Battle
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] 
        private PlayerDefinition _playerDefinition;
        [SerializeField] 
        private CharacterView _playerView;
        [SerializeField] 
        private CardPlayController _playController;
        [SerializeField] 
        private HandController _handController;
        [SerializeField] 
        private Button _endTurnButton;
        [SerializeField] 
        private List<EnemyView> _enemyViews = new();
        [SerializeField] 
        private float _betweenEnemyDelay = 0.4f;

        private Player _player;
        private readonly List<Enemy> _enemies = new();
        private BattleContext _context;
        private CardEffectExecutor _executor;
        private readonly CompositeDisposable _scope = new();

        private bool _enemyPhase;        
        private bool _battleOver;

        private void Start()
        {
            _player = new Player(_playerDefinition);
            if (_playerView != null) _playerView.Bind(_player);

            foreach (var ev in _enemyViews)
            {
                if (ev == null) continue;
                var enemy = ev.Create();
                _enemies.Add(enemy);
                enemy.Died.Subscribe(_ => OnEnemyDied()).AddTo(_scope);
            }

            _context = new BattleContext(_player, _enemies, null);
            _executor = new CardEffectExecutor(_context);
            _player.Activate(_context);

            _playController.CardPlayed
                .Subscribe(play => _executor.Execute(play.Card, play.Target))
                .AddTo(_scope);

            _player.Died.Subscribe(_ => EndBattle(false)).AddTo(_scope);

            if (_endTurnButton != null)
                _endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

        private void OnEndTurnClicked()
        {
            if (_enemyPhase || _battleOver) return;
            EndTurnAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid EndTurnAsync(CancellationToken ct)
        {
            _enemyPhase = true;
            SetPlayerInput(false);

            _player.CheckSizeDeath();   
            if (_battleOver) { _enemyPhase = false; return; }

            await EnemyPhaseAsync(ct);
            if (!_battleOver)
                await BeginNewTurnAsync(ct);

            if (!_battleOver) SetPlayerInput(true);
            _enemyPhase = false;
        }

        private async UniTask EnemyPhaseAsync(CancellationToken ct)
        {
            foreach (var ev in _enemyViews)
                if (ev != null && ev.Enemy != null) ev.Enemy.ClearShield();

            foreach (var ev in _enemyViews)
            {
                if (_battleOver) return;
                var enemy = ev != null ? ev.Enemy : null;
                if (enemy == null || enemy.IsDead) continue;

                await enemy.ActAsync(_context);  
                ev.ClearIntent();                

                if (_player.IsDead) { EndBattle(false); return; }
                await UniTask.Delay(TimeSpan.FromSeconds(_betweenEnemyDelay), cancellationToken: ct);
            }
        }

        private async UniTask BeginNewTurnAsync(CancellationToken ct)
        {
            foreach (var ev in _enemyViews)
                if (ev != null && ev.Enemy != null) ev.Enemy.CheckSizeDeath();

            if (_context.AliveEnemyCount == 0) { EndBattle(true); return; }

            _player.ClearShield();
            _player.ClearTemporaryStatuses();
            foreach (var ev in _enemyViews)
                if (ev != null && ev.Enemy != null && !ev.Enemy.IsDead)
                    ev.Enemy.ClearTemporaryStatuses();

            foreach (var ev in _enemyViews)
                if (ev != null && ev.Enemy != null && !ev.Enemy.IsDead)
                    ev.Replan();

            if (_handController != null)
                await _handController.NewTurnAsync(ct);
        }

        private void OnEnemyDied()
        {
            if (!_battleOver && _context != null && _context.AliveEnemyCount == 0)
                EndBattle(true);
        }

        private void SetPlayerInput(bool on)
        {
            if (_playController != null) _playController.Interactable = on;
            if (_endTurnButton != null) _endTurnButton.interactable = on;
        }

        private void EndBattle(bool win)
        {
            if (_battleOver) return;
            _battleOver = true;
            SetPlayerInput(false);
        }

        private void OnDestroy()
        {
            if (_endTurnButton != null) _endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
            _scope.Dispose();
            _player?.Dispose();
            foreach (var e in _enemies) e?.Dispose();
        }
    }
}
