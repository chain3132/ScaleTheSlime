using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.UI.Card.Data;
using Gameplay.BattleEncounter.UI.Card.Enum;
using Gameplay.BattleEncounter.UI.CardPlie;
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

        public async UniTask SetupAsync(IReadOnlyList<CardDefinition> cards, CancellationToken ct)
        {
            await _rack.InitializeAsync(ct);   

            ClearBattle();                     
            _deck = new Deck(cards, _seed);
            _pileViewModel = new CardPileViewModel(_deck);
            _pileView.Bind(_pileViewModel);

            for (int i = 0; i < _startingHand; i++)
            {
                Data.Card card = _deck.Draw();
                if (card == null) break;
                await _rack.DrawAsync(new CardViewModel(card), ct);
            }
        }

        public void ClearBattle()
        {
            _rack.ClearHand();
            _pileViewModel?.Dispose();
            _pileViewModel = null;
            _deck?.Dispose();
            _deck = null;
        }

        public async UniTask NewTurnAsync(CancellationToken ct)
        {
            if (_deck == null) return;

            await _rack.DiscardHandAsync(IsRetain, ct);   
            _deck.DiscardHandExcept(IsRetain);            

            int toDraw = _startingHand - _deck.Hand.Count;
            for (int i = 0; i < toDraw; i++)
            {
                Data.Card card = _deck.Draw();
                if (card == null) break;
                await _rack.DrawAsync(new CardViewModel(card), ct);
            }
        }

        private static bool IsRetain(Data.Card card)
            => (card.Definition.Keywords & CardKeyword.Retain) != 0;

        private void Bind()
        {
            _playController.CardPlayed
                .SubscribeAwait(async (play, ct) =>
                {
                    await _rack.PlayCardAsync(play.Card, ct);
                    _deck?.Discard(play.Card);
                }, AwaitOperation.Drop)
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _pileViewModel?.Dispose();
            _deck?.Dispose();
        }
    }
}
