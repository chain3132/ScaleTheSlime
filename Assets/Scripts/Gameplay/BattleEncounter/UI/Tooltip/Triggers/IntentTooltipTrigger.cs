using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Status;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{

    public class IntentTooltipTrigger : TooltipTrigger
    {
        [SerializeField] 
        private StatusDatabase _statusDatabase;
        [SerializeField] 
        private string _title = "INTENT";

        private EnemyAction _action;

        public void SetAction(EnemyAction action) => _action = action;

        protected override bool TryBuild(out TooltipData data)
        {
            if (_action == null) { data = default; return false; }
            data = TooltipData.Standard(_title, Describe(_action));
            return true;
        }

        private string Describe(EnemyAction a)
        {
            string status = _statusDatabase != null ? _statusDatabase.NameFor(a.Status) : a.Status.ToString();
            switch (a.Type)
            {
                case EnemyActionType.Attack:
                    return a.Repeat > 1
                        ? $"Attack for {a.Value} damage {a.Repeat} times."
                        : $"Attack for {a.Value} damage.";
                case EnemyActionType.GainArmor:    return $"Gain {a.Value} Block.";
                case EnemyActionType.GrowSize:     return $"Grow size by {a.Value}.";
                case EnemyActionType.ShrinkSize:   return $"Shrink size by {a.Value}.";
                case EnemyActionType.BuffSelf:     return $"Gain {a.Value} {status}.";
                case EnemyActionType.DebuffPlayer: return $"Apply {a.Value} {status} to you.";
                default:                           return string.Empty;
            }
        }
    }
}
