using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Battle;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Behaviors
{
    [CreateAssetMenu(menuName = "Battle/Behavior/Attack")]
    public class AttackBehavior : EnemyBehavior
    {
        [SerializeField] private int _damage = 5;

        public override UniTask Act(Enemy self, BattleContext ctx)
        {
            ctx.Player.TakeDamage(_damage);
            return UniTask.CompletedTask;
        }
    }
}
