using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Characters.Enums;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Data
{
    [CreateAssetMenu(menuName = "Battle/EnemyDefinition")]
    public class EnemyDefinition : CharacterDefinition
    {
        [Header("Behavior ")]
        public EnemyBehavior TinyBehavior;
        public EnemyBehavior NormalBehavior;
        public EnemyBehavior GiantBehavior;

        public EnemyBehavior BehaviorFor(SizeForm form)
        {
            switch (form)
            {
                case SizeForm.Tiny:
                    return TinyBehavior;
                case SizeForm.Normal:
                    return NormalBehavior;
                case SizeForm.Giant:
                    return GiantBehavior;
                default:
                    return null;
            }
        }
    }
}
