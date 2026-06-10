using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Characters.Data;

namespace Gameplay.BattleEncounter.Characters
{
    public class Enemy : Character
    {
        private readonly EnemyDefinition _def;
        public EnemyDefinition Definition => _def;

        public IReadOnlyList<EnemyAction> CurrentPlan { get; private set; } = Array.Empty<EnemyAction>();

        public Enemy(EnemyDefinition def)
            : base(def.DisplayName, def.MaxHealth, def.StartSize)
        {
            _def = def;
        }

        public void PlanTurn()
        {
            var behavior = _def.BehaviorFor(Form.CurrentValue);
            CurrentPlan = behavior != null ? behavior.PlanTurn() : Array.Empty<EnemyAction>();
        }

        public UniTask ActAsync(BattleContext ctx)
        {
            if (IsDead) return UniTask.CompletedTask;
            foreach (var a in CurrentPlan)
                EnemyActionRunner.Run(a, this, ctx);
            return UniTask.CompletedTask;
        }
    }
}
