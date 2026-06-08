using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.BattleEncounter.UI.Card
{
    public class CardView : MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region CardView References
        [Header("Card View References")]
        [SerializeField]
        private Image cardBackground;
        [SerializeField]
        private Image cardArt;
        [SerializeField]
        private Image cardHeader;
        [SerializeField]
        private Image cardIcon;
        [SerializeField]
        private Image cardFrameLeft;
        [SerializeField]
        private Image cardIconRight;
        [SerializeField]
        private TMP_Text headerText;

        #endregion

        public Data.Card Card => _card;   

        #region fields

        private CardPlayController _cardPlayController;
        private Data.Card _card;

        #endregion

        public void Bind(CardViewModel vm,CardPlayController playController)
        {
            _cardPlayController = playController;
            _card = vm.Card;
            var s = vm.Definition.Art;
            if (s == null) return;

            Apply(cardBackground, s.CardBackground, true);
            Apply(cardArt, s.CardArt, true);
            Apply(cardHeader, s.CardHeader, true);
            if (headerText != null) headerText.text = vm.DisplayName;

            Apply(cardIcon, s.CardIcon, s.IsSpecialCard);
            Apply(cardFrameLeft, s.CardFrameLeft, s.IsSpecialCard);
            Apply(cardIconRight, s.CardFrameRight, s.IsSpecialCard);
        }

        private static void Apply(Image image, Sprite sprite, bool visible)
        {
            if (image == null) return;
            image.gameObject.SetActive(visible);
            if (visible) image.sprite = sprite;
        }
        

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Card Hovered");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Card Unhovered");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _cardPlayController.BeginDrag(this.transform.position,_card);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _cardPlayController.OnDrag(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _cardPlayController.EndDrag(eventData.position);
        }
    }
}
