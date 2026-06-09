using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Characters.Data;

namespace Gameplay.BattleEncounter.Characters
{
    public class Enemy : Character
    {
        private readonly EnemyDefinition _def;
        public EnemyDefinition Definition => _def;

        public Enemy(EnemyDefinition def)
            : base(def.DisplayName, def.MaxHealth, def.StartSize)
        {
            _def = def;
        }

        // รัน behavior ตาม form ปัจจุบัน (เรียกตอน player จบเทิร์น)
        public async UniTask ActAsync(BattleContext ctx)
        {
            if (IsDead) return;
            var behavior = _def.BehaviorFor(Form.CurrentValue);
            if (behavior != null) await behavior.Act(this, ctx);
        }
    }
}
