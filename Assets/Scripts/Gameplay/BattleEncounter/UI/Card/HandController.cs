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
        private DeckDefinition _startingDeck;
        [SerializeField] 
        private int _seed = 12345;
        [SerializeField] 
        private int _startingHand = 5;

        private Deck _deck;
        private CardPileViewModel _pileViewModel;
        
        private async UniTaskVoid Start()
        {
            var ct = this.GetCancellationTokenOnDestroy();

            await _rack.InitializeAsync(ct);
            _deck =  new Deck(_startingDeck.Cards, _seed);
            _pileViewModel = new CardPileViewModel(_deck);
            _pileView.Bind(_pileViewModel);
            Bind();
            for (int i = 0; i < _startingHand; i++)
            {
                Data.Card card = _deck.Draw();
                if (card == null) break;
                var vm = new CardViewModel(card);
                await _rack.DrawAsync(vm, ct);
            }
        }

        public async UniTask NewTurnAsync(CancellationToken ct)
        {
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
                    _deck.Discard(play.Card);                    
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
