using Gameplay.BattleEncounter.Battle;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Passives
{
    public abstract class Passive : ScriptableObject
    {
        [SerializeField, TextArea]
        private string _description;
        public string Description => _description;

        public abstract void OnEnter(Character characters, BattleContext ctx);
        public abstract void OnExit(Character characters, BattleContext ctx);
    }
}
