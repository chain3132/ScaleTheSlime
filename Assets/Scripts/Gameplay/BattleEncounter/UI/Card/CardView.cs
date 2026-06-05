using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.BattleEncounter.UI.Card
{
    public class CardView : MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler
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

        public void Bind(CardViewModel vm)
        {
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

        public void TestClick()
        {
            Debug.Log("Card Clicked");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Card Hovered");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Card Unhovered");
        }
    }
}
