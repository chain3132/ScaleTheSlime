using System;
using System.Collections.Generic;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.UI.Card.Data;
using R3;

namespace Gameplay.BattleEncounter.Battle
{
    public class BattleContext : IDisposable
    {
        #region Properties
        public Player Player { get; }
        public IReadOnlyList<Enemy> Enemies => _enemies;
        public Deck Deck { get; }
        public int AliveEnemyCount {
            get
            {
                int n = 0;
                foreach (var e in _enemies)
                {
                    if (!e.IsDead) n++;
                }
                return n;
            }
        }

        public Observable<int> DrawRequested => _drawRequested;
        public Observable<Unit> DiscardRequested => _discardRequested;
        public Observable<int> ChooseDiscardRequested => _chooseDiscardRequested;
        #endregion

        #region fields

        private readonly List<Enemy> _enemies;
        private readonly Subject<int> _drawRequested = new();
        private readonly Subject<Unit> _discardRequested = new();
        private readonly Subject<int> _chooseDiscardRequested = new();


        #endregion

        public BattleContext(Player player, List<Enemy> enemies, Deck deck)
        {
            Player = player;
            _enemies = enemies;
            Deck = deck;
        }

        public void RequestDraw(int count)
        {
            if (count > 0) _drawRequested.OnNext(count);
        }
        public void RequestDiscard()
        {
            _discardRequested.OnNext(Unit.Default);
        }
        public void RequestChooseDiscard(int count)
        {
            if (count > 0) _chooseDiscardRequested.OnNext(count);
            
        }

        public void Dispose()
        {
            _drawRequested.Dispose();
            _discardRequested.Dispose();
            _chooseDiscardRequested.Dispose();
        }
    }
}
