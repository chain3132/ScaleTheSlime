using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.UI.Card.Data;
using Gameplay.BattleEncounter.UI.Card.Enum;
using Gameplay.BattleEncounter.UI.CardPile;
using R3;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    public class HandController : MonoBehaviour
    {
        [SerializeField]
        private CardRack _rack;
        [SerializeField]
        private CardPlayController _playController;
        [SerializeField]
        private CardPileView _pileView;
        [SerializeField]
        private int _seed = 12345;
        [SerializeField]
        private int _startingHand = 5;

        private Deck _deck;
        private CardPileViewModel _pileViewModel;

        public Deck Deck => _deck;   

        private void Start() => Bind();
        private static bool IsRetain(Data.Card card)
        {
            return (card.Definition.Keywords & CardKeyword.Retain) != 0;
        }
        private static bool IsDestroyOnPlay(Data.Card card)
        {
            return (card.Definition.Keywords & CardKeyword.DestroyOnPlay) != 0;
        }

        private void Bind()
        {
            _playController.CardPlayed
                .SubscribeAwait(async (play, ct) =>
                {
                    await _rack.PlayCardAsync(play.Card, ct);
                    if (IsDestroyOnPlay(play.Card))
                        _deck?.RemoveFromHand(play.Card);   
                    else
                        _deck?.Discard(play.Card);
                }, AwaitOperation.Drop)
                .AddTo(this);
        }
        
        public async UniTask SetupAsync(IReadOnlyList<CardDefinition> cards, CancellationToken ct)
        {
            await _rack.InitializeAsync(ct);   

            ClearBattle();                     
            _deck = new Deck(cards, _seed);
            _pileViewModel = new CardPileViewModel(_deck);
            _pileView.Bind(_pileViewModel);

            await DrawCardsAsync(_startingHand, ct);
        }
        public async UniTask ChooseDiscardThenDrawAsync(int count, CancellationToken ct)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (_deck.Hand.Count == 0) break;
                    if (_rack.HandCount == 0) break;
                    _playController.SelectionMode = true;

                    var picked = await _playController.CardClicked.FirstAsync(ct);
                    _playController.SelectionMode = false;
                    _deck.Discard(picked);
                    await _rack.DiscardOneAsync(picked, ct);
                }
            }
            finally
            {
                _playController.SelectionMode = false; 
            }
            await DrawCardsAsync(count, ct);
        }
        
        public async UniTask DrawCardsAsync(int count, CancellationToken ct)
        {
            for (int i = 0; i < count; i++)
            {
                Data.Card card = _deck?.Draw();
                if (card == null) break;
                await _rack.DrawAsync(new CardViewModel(card), ct);
            }
        }

        public async UniTask AddUniqueCardAsync(CardDefinition def, CancellationToken ct)
        {
            if (_deck == null || def == null) return;
            var card = _deck.AddToHand(def);
            if (card == null) return;
            await _rack.DrawAsync(new CardViewModel(card), ct);
        }

        public async UniTask DiscardHandVisualAsync(CancellationToken ct)
        {
            await _rack.DiscardHandAsync(null, ct);
        }

        public async UniTask NewTurnAsync(CancellationToken ct)
        {
            if (_deck == null) return;

            await _rack.DiscardHandAsync(IsRetain, ct);   
            _deck.DiscardHandExcept(IsRetain);            

            await DrawCardsAsync(_startingHand - _deck.Hand.Count, ct);
        }
        public void ClearBattle()
        {
            _rack.ClearHand();
            _pileViewModel?.Dispose();
            _pileViewModel = null;
            _deck?.Dispose();
            _deck = null;
        }
        
        

        private void OnDestroy()
        {
            _pileViewModel?.Dispose();
            _deck?.Dispose();
        }
    }
}
