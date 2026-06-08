using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.UI.Card.Data;
using Gameplay.BattleEncounter.UI.CardPlie;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.BattleEncounter.UI.Card
{
    public class HandController : MonoBehaviour
    {
        [SerializeField] 
        private CardRack _rack;
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
            
            
            for (int i = 0; i < _startingHand; i++)
            {
                Data.Card card = _deck.Draw();
                if (card == null) break;
                var vm = new CardViewModel(card);    
                await _rack.DrawAsync(vm, ct);
            }
        }
    }
}
