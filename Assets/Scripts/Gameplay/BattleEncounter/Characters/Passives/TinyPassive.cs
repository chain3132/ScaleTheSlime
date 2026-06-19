using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Status;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Passives
{
    [CreateAssetMenu(menuName = "Battle/Passive/TinyPassive")]

    public class TinyPassive : Passive
    {
        [SerializeField]
        private int _amount = 1;
        
        public override void OnEnter(Character character, BattleContext ctx)
        {
            character.ApplyStatus(StatusType.ShieldBonus,_amount,persistent:true);
        }

        public override void OnExit(Character character, BattleContext ctx)
        {
            character.ApplyStatus(StatusType.ShieldBonus,-_amount,persistent:true);        
        }
    }
}
