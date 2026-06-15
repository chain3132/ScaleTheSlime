using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Reward
{
    [CreateAssetMenu(menuName = "Battle/CardRewardPool")]
    public class CardRewardPool : ScriptableObject
    {
        [SerializeField]
        private List<Card.CardDefinition> _cards = new();

        public List<Card.CardDefinition> Roll(int count, System.Random rng)
        {
            var pool = new List<Card.CardDefinition>(_cards);
            var result = new List<Card.CardDefinition>();
            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int idx = rng.Next(pool.Count);
                result.Add(pool[idx]);
                pool.RemoveAt(idx);
            }
            return result;
        }
    }
}
