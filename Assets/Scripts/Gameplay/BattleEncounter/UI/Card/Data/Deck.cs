using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card.Data
{
    public class Deck : IDisposable
    {
        private readonly List<Card> _drawPile = new();
        private readonly List<Card> _discardPile = new();
        private readonly List<Card> _hand = new();
        private readonly System.Random _rng;
        private readonly ReactiveProperty<int> _drawCount = new(0);
        private readonly ReactiveProperty<int> _discardCount = new(0);
        
        public IReadOnlyList<Card> DrawPile => _drawPile;
        public IReadOnlyList<Card> Hand => _hand;
        public IReadOnlyList<Card> DiscardPile => _discardPile;
        

        public ReadOnlyReactiveProperty<int> DrawCount => _drawCount;    
        public ReadOnlyReactiveProperty<int> DiscardCount => _discardCount;

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
            SyncCounts();
            return card;
        }

        public void Discard(Card card)
        {
            if (_hand.Remove(card))
                _discardPile.Add(card);
            SyncCounts();

        }

        public void DiscardHand()
        {
            _discardPile.AddRange(_hand);
            _hand.Clear();
            SyncCounts();
        }

        private void ReshuffleDiscardIntoDraw()
        {
            if (_discardPile.Count == 0) return;
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            SyncCounts();
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
        
        private void SyncCounts()
        {
            _drawCount.Value = _drawPile.Count;
            _discardCount.Value = _discardPile.Count;
        }

        public void Dispose()
        {
            _drawCount.Dispose();
            _discardCount.Dispose();
        }
    }
}
