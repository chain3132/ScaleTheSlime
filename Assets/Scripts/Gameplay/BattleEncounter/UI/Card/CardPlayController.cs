using System;
using Gameplay.BattleEncounter.UI.Card.Enum;
using R3;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    public class CardPlayController : MonoBehaviour
    {
        #region References

        [SerializeField] 
        private RectTransform _arrowRT;
        [SerializeField] 
        private GameObject _playZoneUI;

        #endregion

        #region Setup

        [SerializeField]
        private int thickness;
        [SerializeField] 
        private RectTransform _playZoneRect;

        #endregion

        #region Fields

        private Vector3 _origin;
        private Data.Card _currentCard;
        private readonly Subject<Data.Card> _cardPlayed = new();

        #endregion

        public Observable<Data.Card> CardPlayed => _cardPlayed; 
        
        public void BeginDrag(Vector2 cardPosition,Data.Card card)
        {
            _currentCard = card;
            _origin = cardPosition;
            _arrowRT.gameObject.SetActive(true);
        }
        public void OnDrag(Vector2 mousePosition)
        {
            
            UpdateArrow(mousePosition, _origin);
            IfInPlayZone(mousePosition);
        }
        public void EndDrag(Vector2 mousePosition)
        {
            _arrowRT.gameObject.SetActive(false);
            _playZoneUI.SetActive(false);

            if (IsValidPlay(_currentCard, mousePosition))
            {
                _cardPlayed.OnNext(_currentCard);;   

            }
            _currentCard = null;
        }

        private bool IsValidPlay(Data.Card card, Vector2 mousePosition)
        {
            if (card == null) return false;
            switch (card.Definition.PlayTarget)
            {
                case CardTarget.Player:
                case CardTarget.Background:
                    return RectTransformUtility.RectangleContainsScreenPoint(_playZoneRect, mousePosition, null);
                case CardTarget.Enemy:
                    return false; 
                default:
                    return false; // None 
            }
        }

        private void UpdateArrow(Vector2 mousePosition, Vector2 cardPosition)
        {
            var parent = (RectTransform)_arrowRT.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, cardPosition, null, out Vector2 a);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, mousePosition, null, out Vector2 b);

            Vector2 dir = b - a;
            _arrowRT.sizeDelta = new Vector2(thickness, dir.magnitude);
            _arrowRT.anchoredPosition = a + dir * 0.5f;
            _arrowRT.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90);
        }

        private void IfInPlayZone(Vector2 mousePosition)
        {
            bool isInPlayZone = RectTransformUtility.RectangleContainsScreenPoint(_playZoneRect, mousePosition, null);
            if (isInPlayZone)
            {
                CheckTypeCard(_currentCard.Definition.PlayTarget);
                return;
            }
            else
            {
                _playZoneUI.SetActive(false);

            }
            
        }

        private void CheckTypeCard(CardTarget target)
        {
            switch (target)
            {
                case CardTarget.Background:
                case CardTarget.Player:
                    _playZoneUI.SetActive(true);
                    break;
                default:
                    _playZoneUI.SetActive(false);
                    break;
                    
            }
        }

        private void OnDestroy()
        {
            _cardPlayed.Dispose();
        }
    }
}
