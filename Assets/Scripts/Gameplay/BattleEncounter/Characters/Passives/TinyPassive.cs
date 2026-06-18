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
        public override void OnEnter(Player player, BattleContext ctx)
        {
            player.ApplyStatus(StatusType.ShieldBonus,_amount,persistent:true);
        }

        public override void OnExit(Player player, BattleContext ctx)
        {
            player.ApplyStatus(StatusType.ShieldBonus,-_amount,persistent:true);
        }
    }
}
