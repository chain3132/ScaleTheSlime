using Gameplay.BattleEncounter.UI.Card.Data;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    public class CardViewModel
    {
        public Data.Card Card { get; }
        public CardDefinition Definition => Card.Definition;
        public string DisplayName => Definition.DisplayName;
        public Sprite Art => Definition.Art != null ? Definition.Art.CardArt : null;

        public CardViewModel(Data.Card card)
        {
            Card = card;
        }
    }
}
