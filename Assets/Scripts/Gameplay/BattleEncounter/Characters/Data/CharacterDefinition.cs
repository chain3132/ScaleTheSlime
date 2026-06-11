using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Data
{
    public abstract class CharacterDefinition : ScriptableObject
    {
        public int MaxHealth = 30;
        [Range(0, 10)] 
        public int StartSize = 5;
    }
}
