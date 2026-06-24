using Gameplay.BattleEncounter.UI.Card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BattleEncounter.UI.Reward
{
    
    public class CardRewardView : MonoBehaviour
    {
        #region References

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
        private Image cardFrameRight;
        [SerializeField]
        private Image cardBorderTop;
        [SerializeField]
        private TMP_Text headerText;
        [SerializeField] 
        private TMP_Text descriptionText;

        #endregion

        public void Bind(CardDefinition definition)
        {
            if (definition == null) return;
            var s = definition.Art;
            if (s == null) return;

            Apply(cardBackground, s.CardBackground, true);
            Apply(cardArt, s.CardArt, true);
            Apply(cardHeader, s.CardHeader, true);
            if (headerText != null) headerText.text = definition.DisplayName;
            ApplyDescription(definition);
            Apply(cardIcon, s.CardIcon, s.IsSpecialCard);
            Apply(cardFrameLeft, s.CardFrameLeft, s.IsSpecialCard);
            Apply(cardFrameRight, s.CardFrameRight, s.IsSpecialCard);
            Apply(cardBorderTop, s.CardBorderTop, s.IsSpecialCard);
        }

        private static void Apply(Image image, Sprite sprite, bool visible)
        {
            if (image == null) return;
            image.gameObject.SetActive(visible);
            if (visible) image.sprite = sprite;
        }

        private void ApplyDescription(CardDefinition cardDefinition)
        {
            if (descriptionText != null)
                descriptionText.text = CardTextFormatter.Format(cardDefinition);
        }
        
        
    }
}
