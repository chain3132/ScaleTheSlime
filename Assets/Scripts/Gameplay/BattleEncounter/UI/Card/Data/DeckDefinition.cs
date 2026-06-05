using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card.Data
{
    [CreateAssetMenu(menuName = "Battle/DeckDefinition")]
    public class DeckDefinition : ScriptableObject
    {
        public List<CardDefinition> Cards = new();
    }
}
