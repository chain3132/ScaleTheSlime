using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Status;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Passives
{
    [CreateAssetMenu(menuName = "Battle/Passive/GiantPassive")]
    public class GiantPassive : Passive
    {
        [SerializeField] private int _amount = 2;

        public override void OnEnter(Player player, BattleContext ctx)
        {
            foreach (var enemy in ctx.Enemies)
            {
                enemy.ApplyStatus(StatusType.Vulnerable,_amount,persistent:true);
            }
        }
        public override void OnExit(Player player, BattleContext ctx)
        {
            foreach (var enemy in ctx.Enemies)
            {
                enemy.ApplyStatus(StatusType.Vulnerable,-_amount,persistent:true);
            }        
        }
        
    }
}
