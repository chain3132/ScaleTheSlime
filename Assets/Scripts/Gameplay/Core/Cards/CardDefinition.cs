using System.Collections.Generic;
using Gameplay.BattleEncounter.UI.Card.Enum;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    [CreateAssetMenu(menuName = "Battle/CardDefinition")]
    public class CardDefinition : ScriptableObject
    {
        [Required("DisplayName is missing")]
        public string DisplayName;
        
        [ResizableTextArea,] 
        public string Description;
        [Required("Art is missing")]
        public CardSprites Art;
        [Required("CardTarget is missing")]
        public CardTarget PlayTarget;
        [Required("CardKeyword is missing")]
        public CardKeyword Keywords;        
        public List<CardEffectData> Effects;
    }
}
