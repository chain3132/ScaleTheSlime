using System.Collections.Generic;
using Gameplay.BattleEncounter.UI.Card.Enum;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    [CreateAssetMenu(menuName = "Battle/CardDefinition")]
    public class CardDefinition : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public CardSprites Art;
        public CardTarget PlayTarget;          
        public CardKeyword Keywords;        
        public List<CardEffectData> Effects;
    }
}
