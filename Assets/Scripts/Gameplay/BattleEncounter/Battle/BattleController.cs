using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Characters;
using Gameplay.NodeSelection.UI;
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
        [Header("Enemy spawning")]
        [SerializeField]
        private EnemyView _enemyViewPrefab;
        [SerializeField]
        private Transform _enemyContainer;
        [SerializeField]
        private int _maxEnemies = 3;
        [SerializeField]
        private float _enemySpacing = 3f;   
        [SerializeField]
        private float _betweenEnemyDelay = 0.4f;
        [SerializeField]
        private bool _autoStart = true;

        private Player _player;
        private readonly List<EnemyView> _enemyViews = new(); 
        private readonly List<Enemy> _enemies = new();
        private BattleContext _context;
        private CardEffectExecutor _executor;
        private readonly CompositeDisposable _battleScope = new();   
        private UniTaskCompletionSource<bool> _result;               

        private bool _enemyPhase;
        private bool _battleOver;

        private void Start()
        {
            if (_endTurnButton != null)
                _endTurnButton.onClick.AddListener(OnEndTurnClicked);

            if (_autoStart)
            {
                var deck = _playerDefinition.StartingDeck != null
                    ? new List<CardDefinition>(_playerDefinition.StartingDeck.Cards)
                    : new List<CardDefinition>();
                var progress = new RunProgress(_playerDefinition.MaxHealth, deck);
                RunAsync(progress, this.GetCancellationTokenOnDestroy()).Forget();
            }
        }
        public async UniTask<bool> RunAsync(RunProgress progress, CancellationToken ct)
            => await RunAsync(progress, null, ct);

        public async UniTask<bool> RunAsync(RunProgress progress,
            IReadOnlyList<EnemyDefinition> enemyDefs, CancellationToken ct)
        {
            Setup(progress, enemyDefs);
            if (_handController != null)
                await _handController.SetupAsync(progress.Deck, ct);   

            bool win = await _result.Task.AttachExternalCancellation(ct);

            if (win) progress.SetHealth(_player.Health.Value);
            Teardown();
            return win;
        }

        private void Setup(RunProgress progress, IReadOnlyList<EnemyDefinition> enemyDefs)
        {
            _battleOver = false;
            _enemyPhase = false;
            _result = new UniTaskCompletionSource<bool>();

            _player = new Player(_playerDefinition, progress.CurrentHealth);
            if (_playerView != null) _playerView.Bind(_player, _playerDefinition);

            SpawnEnemies(enemyDefs);

            _context = new BattleContext(_player, _enemies, 
                _handController != null ? _handController.Deck : null);
            _executor = new CardEffectExecutor(_context);
            _player.Activate(_context);

            _playController.CardPlayed
                .Subscribe(play => _executor.Execute(play.Card, play.Target))
                .AddTo(_battleScope);

            if (_handController != null)
            {
                _context.DrawRequested
                    .SubscribeAwait(async (n, token) => await _handController.DrawCardsAsync(n, token),
                        AwaitOperation.Sequential)
                    .AddTo(_battleScope);
                _context.DiscardRequested
                    .SubscribeAwait(async (_, token) => await _handController.DiscardHandVisualAsync(token),
                        AwaitOperation.Sequential)
                    .AddTo(_battleScope);
                _context.ChooseDiscardRequested
                    .SubscribeAwait(async (n, token) =>
                        {
                            SetPlayerInput(false);
                            try
                            {
                                await _handController.ChooseDiscardThenDrawAsync(n, token);
                            }
                            finally
                            {
                                _playController.SelectionMode = false;                     
                                if (!_battleOver && !_enemyPhase) SetPlayerInput(true);
                            }
                        },
                        AwaitOperation.Sequential)
                    .AddTo(_battleScope);

                _player.UniqueCardGranted
                    .SubscribeAwait(async (def, token) => await _handController.AddUniqueCardAsync(def, token),
                        AwaitOperation.Sequential)
                    .AddTo(_battleScope);
            }

            _player.Died.Subscribe(_ => EndBattle(false)).AddTo(_battleScope);

            SetPlayerInput(true);
        }

        private void SpawnEnemies(IReadOnlyList<EnemyDefinition> enemyDefs)
        {
            if (_enemyViewPrefab == null) return;

            var defs = new List<EnemyDefinition>();
            if (enemyDefs == null)
            {
                defs.Add(null);
            }
            else
            {
                foreach (var d in enemyDefs)
                {
                    if (d == null) continue;
                    defs.Add(d);
                    if (defs.Count >= _maxEnemies) break;
                }
            }

            int n = defs.Count;
            var parent = _enemyContainer != null ? _enemyContainer : transform;

            for (int i = 0; i < n; i++)
            {
                var ev = Instantiate(_enemyViewPrefab, parent);
                ev.transform.localPosition = new Vector3((i - (n - 1) / 2f) * _enemySpacing, 0f, 0f);

                var enemy = ev.Setup(defs[i]);
                if (enemy == null) { Destroy(ev.gameObject); continue; }

                _enemyViews.Add(ev);
                _enemies.Add(enemy);
                enemy.Died.Subscribe(_ => OnEnemyDied()).AddTo(_battleScope);
            }
        }

        private void Teardown()
        {
            _battleScope.Clear();
            _player?.Dispose();
            _player = null;
            foreach (var e in _enemies) e?.Dispose();
            _enemies.Clear();
            foreach (var ev in _enemyViews)
                if (ev != null) Destroy(ev.gameObject);   
            _enemyViews.Clear();
            _handController?.ClearBattle();
            _context?.Dispose();
            _context = null;
            _executor = null;
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
            _result?.TrySetResult(win);
        }

        private void OnDestroy()
        {
            if (_endTurnButton != null) _endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
            _battleScope.Dispose();
            _player?.Dispose();
            foreach (var e in _enemies) e?.Dispose();
            _context?.Dispose();
        }
    }
}
