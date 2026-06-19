using Gameplay.BattleEncounter.Battle;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Passives
{
    public abstract class Passive : ScriptableObject
    {
        public abstract void OnEnter(Character characters, BattleContext ctx);
        public abstract void OnExit(Character characters, BattleContext ctx);
    }
}
