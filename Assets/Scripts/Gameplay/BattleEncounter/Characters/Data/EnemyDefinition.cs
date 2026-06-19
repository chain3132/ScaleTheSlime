using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Characters.Enums;
using Gameplay.BattleEncounter.Characters.Passives;
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
        
        [Header("Passive")]
        public Passive TinyPassive;
        public Passive NormalPassive;
        public Passive GiantPassive;

        [Header("Size Death")]
        [SerializeField] 
        private bool _diesWhenTooSmall = true;   // size = 0 → ตาย
        [SerializeField] 
        private bool _diesWhenTooBig = true;     // size = 10 → ตาย
        public bool DiesWhenTooSmall => _diesWhenTooSmall;
        public bool DiesWhenTooBig => _diesWhenTooBig;

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
        public Passive PassiveFor(SizeForm form) 
        {
            switch (form)
            {
                case SizeForm.Tiny:
                    return TinyPassive;
                case SizeForm.Normal:
                    return NormalPassive;
                case SizeForm.Giant:
                    return GiantPassive;
                default:
                    return null;
            }
        }
    }
}
