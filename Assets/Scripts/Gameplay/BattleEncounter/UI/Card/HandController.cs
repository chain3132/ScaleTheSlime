using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.UI.Card.Data;
using Gameplay.BattleEncounter.UI.CardPlie;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

        private void Bind()
        {
            _playController.CardPlayed
                .SubscribeAwait(async (card, ct) =>
                {
                    await _rack.PlayCardAsync(card, ct);
                    _deck.Discard(card);
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
