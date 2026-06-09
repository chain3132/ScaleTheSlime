using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Status;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Passives
{
    [CreateAssetMenu(menuName = "Battle/Passive/Strength")]
    public class StrengthPassive : Passive
    {
        [SerializeField] private int _amount = 2;

        public override void OnEnter(Player player, BattleContext ctx)
            => player.ApplyStatus(StatusType.Strength, _amount);

        public override void OnExit(Player player, BattleContext ctx)
            => player.ApplyStatus(StatusType.Strength, -_amount);
    }
}
