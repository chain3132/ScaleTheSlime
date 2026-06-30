using Gameplay.BattleEncounter.Battle;
using UnityEngine;
using UnityEngine.Localization;

namespace Gameplay.BattleEncounter.Characters.Passives
{
    public abstract class Passive : ScriptableObject
    {
        [SerializeField]
        private LocalizedString _description;
        public string Description => _description.GetLocalizedString();

        public abstract void OnEnter(Character characters, BattleContext ctx);
        public abstract void OnExit(Character characters, BattleContext ctx);
    }
}
