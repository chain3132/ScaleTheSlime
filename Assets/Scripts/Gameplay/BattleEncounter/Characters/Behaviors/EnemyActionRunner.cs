using Gameplay.BattleEncounter.Battle;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Behaviors
{
    public static class EnemyActionRunner
    {
        public static void Run(EnemyAction a, Enemy self, BattleContext ctx)
        {
            int times = Mathf.Max(1, a.Repeat);
            for (int i = 0; i < times; i++)
            {
                switch (a.Type)
                {
                    case EnemyActionType.Attack:       ctx.Player.TakeDamage(a.Value,self); break;
                    case EnemyActionType.GainArmor:    self.GainShield(a.Value); break;
                    case EnemyActionType.GrowSize:     self.ChangeSize(a.Value); break;
                    case EnemyActionType.ShrinkSize:   self.ChangeSize(-a.Value); break;
                    case EnemyActionType.BuffSelf:     self.ApplyStatus(a.Status, a.Value); break;
                    case EnemyActionType.DebuffPlayer: ctx.Player.ApplyStatus(a.Status, a.Value); break;
                    case EnemyActionType.Heal:         self.Heal(a.Value); break;
                }
            }
        }
    }
}
