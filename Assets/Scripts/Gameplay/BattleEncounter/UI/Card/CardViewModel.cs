using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.UI.Card.Data;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    public class CardViewModel
    {
        public Data.Card Card { get; }
        public BattleContext Context { get; }
        public CardDefinition Definition => Card.Definition;
        public string DisplayName => Definition.DisplayName;

        
        public string Description => Context != null
            ? CardTextFormatter.Format(Definition, Context)
            : Definition.Description;

        public Sprite Art => Definition.Art != null ? Definition.Art.CardArt : null;

        public CardViewModel(Data.Card card, BattleContext context = null)
        {
            Card = card;
            Context = context;
        }
    }
}
