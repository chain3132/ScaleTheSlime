using Gameplay.BattleEncounter.UI.Card;
using R3;
using TMPro;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.CardPile
{
    public class CardPileView : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text drawPileCountText;
        
        [SerializeField]
        private TMP_Text discardPileCountText;
        
        private readonly CompositeDisposable _disposables = new();


        public void Bind(CardPileViewModel vm)
        {
            _disposables.Clear();   
            vm.DrawPileCount.Subscribe(count => drawPileCountText.text = $"{count}").AddTo(_disposables);
            vm.DiscardPileCount.Subscribe(count => discardPileCountText.text = $"{count}").AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
