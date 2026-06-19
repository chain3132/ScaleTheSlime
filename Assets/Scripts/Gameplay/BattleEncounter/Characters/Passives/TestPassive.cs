using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Status;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Passives
{
    [CreateAssetMenu(menuName = "Battle/Passive/TestThornPassive")]
    public class TestPassive : Passive
    {
        [SerializeField]
        private int _amount = 2;

        public override void OnEnter(Character character, BattleContext ctx)
        {
            
            character.ApplyStatus(StatusType.Unstable,_amount,persistent:false);
            
        }

        public override void OnExit(Character character, BattleContext ctx)
        {
            
            character.ApplyStatus(StatusType.Unstable,-_amount,persistent:false);
            
        }
    }
}
