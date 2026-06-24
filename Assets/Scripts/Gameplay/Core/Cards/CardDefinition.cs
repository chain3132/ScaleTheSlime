using System.Collections.Generic;
using Gameplay.BattleEncounter.UI.Card.Enum;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    [CreateAssetMenu(menuName = "Battle/CardDefinition")]
    public class CardDefinition : ScriptableObject
    {
        public string DisplayName;
        [ResizableTextArea] 
        public string Description;
        public CardSprites Art;
        public CardTarget PlayTarget;          
        public CardKeyword Keywords;        
        public List<CardEffectData> Effects;
    }
}
