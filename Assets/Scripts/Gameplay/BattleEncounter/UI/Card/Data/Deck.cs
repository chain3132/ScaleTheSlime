using System.Collections.Generic;

namespace Gameplay.BattleEncounter.UI.Card.Data
{
    public class Deck
    {
        private readonly List<Card> _drawPile = new();
        private readonly List<Card> _discardPile = new();
        private readonly List<Card> _hand = new();
        private readonly System.Random _rng;

        public IReadOnlyList<Card> DrawPile => _drawPile;
        public IReadOnlyList<Card> Hand => _hand;
        public IReadOnlyList<Card> DiscardPile => _discardPile;
        public int DrawCount => _drawPile.Count;

        public Deck(IEnumerable<CardDefinition> startingCards, int seed)
        {
            _rng = new System.Random(seed);
            foreach (var def in startingCards)
                if (def != null) _drawPile.Add(new Card(def));
            Shuffle(_drawPile);
        }

        public Card Draw()
        {
            if (_drawPile.Count == 0) ReshuffleDiscardIntoDraw();
            if (_drawPile.Count == 0) return null;

            int last = _drawPile.Count - 1;     
            var card = _drawPile[last];
            _drawPile.RemoveAt(last);
            _hand.Add(card);
            return card;
        }

        public void Discard(Card card)
        {
            if (_hand.Remove(card))
                _discardPile.Add(card);
        }

        public void DiscardHand()
        {
            _discardPile.AddRange(_hand);
            _hand.Clear();
        }

        private void ReshuffleDiscardIntoDraw()
        {
            if (_discardPile.Count == 0) return;
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            Shuffle(_drawPile);
        }

        private void Shuffle(List<Card> list)
        {
            for (int i = list.Count - 1; i > 0; i--)   // Fisher-Yates
            {
                int j = _rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
