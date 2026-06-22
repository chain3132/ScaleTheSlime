using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.Characters.Enums;
using Gameplay.BattleEncounter.Characters.Passives;

namespace Gameplay.BattleEncounter.Characters
{
    public class Enemy : Character
    {
        private readonly EnemyDefinition _def;
        public EnemyDefinition Definition => _def;

        public IReadOnlyList<EnemyAction> CurrentPlan { get; private set; } = Array.Empty<EnemyAction>();

        public Enemy(EnemyDefinition def)
            : base( def.MaxHealth, def.StartSize)
        {
            _def = def;
        }

        protected override bool IsLethalSize(int size)
            => (_def.DiesWhenTooSmall && size <= 0) || (_def.DiesWhenTooBig && size >= 10);
        
        private Passive _currentPassive;
        private BattleContext _ctx;

        public void Active(BattleContext ctx)
        {
            _ctx = ctx;
            ApplyPassive(Form.CurrentValue);                       

        }

        public void PlanTurn()
        {
            var behavior = _def.BehaviorFor(Form.CurrentValue);
            CurrentPlan = behavior != null ? behavior.PlanTurn() : Array.Empty<EnemyAction>();
        }
        private void ApplyPassive(SizeForm form)
        {
            _currentPassive = _def.PassiveFor(form);
            _currentPassive?.OnEnter(this, _ctx);
        }
        public UniTask ActAsync(BattleContext ctx)
        {
            if (IsDead) return UniTask.CompletedTask;
            foreach (var a in CurrentPlan)
            {
                if (IsDead || ctx.Player.IsDead) break;   
                EnemyActionRunner.Run(a, this, ctx);
            }
            return UniTask.CompletedTask;
        }
    }
}
