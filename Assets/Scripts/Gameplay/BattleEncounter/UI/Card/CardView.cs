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
            if (cardArt != null) cardArt.sprite = vm.Art;
            if (headerText != null) headerText.text = vm.DisplayName;
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
