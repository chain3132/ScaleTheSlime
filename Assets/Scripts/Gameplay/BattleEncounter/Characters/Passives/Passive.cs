using Gameplay.BattleEncounter.Battle;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Passives
{
    public abstract class Passive : ScriptableObject
    {
        public abstract void OnEnter(Player player, BattleContext ctx);
        public abstract void OnExit(Player player, BattleContext ctx);
    }
}
