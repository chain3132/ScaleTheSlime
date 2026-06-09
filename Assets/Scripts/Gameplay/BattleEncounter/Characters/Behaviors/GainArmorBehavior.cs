using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Battle;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Behaviors
{
    [CreateAssetMenu(menuName = "Battle/Behavior/GainArmor")]
    public class GainArmorBehavior : EnemyBehavior
    {
        [SerializeField] private int _armor = 5;

        public override UniTask Act(Enemy self, BattleContext ctx)
        {
            self.GainShield(_armor);
            return UniTask.CompletedTask;
        }
    }
}
