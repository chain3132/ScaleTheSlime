using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Behaviors
{
    [CreateAssetMenu(menuName = "Battle/Behavior/MultiAction")]
    public class MultiActionBehavior : EnemyBehavior
    {
        public enum SelectMode
        {
            DoAll,      
            RandomOne,  
        }

        [SerializeField] private SelectMode _mode = SelectMode.DoAll;
        [SerializeField] private List<EnemyAction> _actions = new();

        public override IReadOnlyList<EnemyAction> PlanTurn()
        {
            if (_actions.Count == 0) return System.Array.Empty<EnemyAction>();

            if (_mode == SelectMode.RandomOne)
                return new[] { _actions[Random.Range(0, _actions.Count)] };

            return _actions; // DoAll
        }
    }
}
