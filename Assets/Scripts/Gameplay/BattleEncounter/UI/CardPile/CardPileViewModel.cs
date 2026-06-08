using System;
using Gameplay.BattleEncounter.UI.Card.Data;
using R3;

namespace Gameplay.BattleEncounter.UI.CardPlie
{
    public class CardPileViewModel : IDisposable
    {
        public ReadOnlyReactiveProperty<int> DrawPileCount { get; }
        public ReadOnlyReactiveProperty<int> DiscardPileCount { get; }
    
        public CardPileViewModel(Deck deck)
        {
            DrawPileCount = deck.DrawCount.ToReadOnlyReactiveProperty();
            DiscardPileCount = deck.DiscardCount.ToReadOnlyReactiveProperty();
        }
        
        public void Dispose()
        {
            DiscardPileCount.Dispose();
            DrawPileCount.Dispose();
        }
    }
}
