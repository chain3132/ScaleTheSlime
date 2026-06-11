using System.Collections.Generic;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.UI.Card.Data;

namespace Gameplay.BattleEncounter.Battle
{
    public class BattleContext
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
        #endregion

        #region fields

        private readonly List<Enemy> _enemies;


        #endregion

        public BattleContext(Player player, List<Enemy> enemies, Deck deck)
        {
            Player = player;
            _enemies = enemies;
            Deck = deck;
        }

        
    }
}
