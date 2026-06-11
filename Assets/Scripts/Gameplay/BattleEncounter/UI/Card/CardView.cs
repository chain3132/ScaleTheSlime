using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
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
        [SerializeField]
        private RectTransform rtRectTransform;

        #endregion

        public Data.Card Card => _card;   

        #region fields
        private MotionHandle _hoverMotion;
        private CardPlayController _cardPlayController;
        private Data.Card _card;
        private bool _isDragging;
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
            if (_isDragging) return;    
            transform.SetAsLastSibling(); 
            if (_hoverMotion.IsActive()) _hoverMotion.Cancel();
            {
                _hoverMotion = LMotion.Create(rtRectTransform.localScale, Vector3.one * 1.1f, 0.15f).WithEase(Ease.OutQuad)
                    .BindToLocalScale(rtRectTransform);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDragging) return;    
            ResetDragVisual();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_cardPlayController == null) return;   
            if (!_cardPlayController.Interactable) return;
            _isDragging = true;
            _cardPlayController.BeginDrag(this.transform.position,_card);
            transform.SetAsLastSibling(); 
            if (_hoverMotion.IsActive()) _hoverMotion.Cancel();
            {
                _hoverMotion = LSequence.Create()
                    .Append(LMotion.Create(rtRectTransform.localScale, Vector3.one * 1.3f , 0.11f)
                        .WithEase(Ease.OutQuad)
                        .BindToLocalScale(rtRectTransform))
                    .Join(LMotion.Create(rtRectTransform.rotation, Quaternion.Euler(0f, 0f, 5f), 0.11f)
                        .WithEase(Ease.OutQuad)
                        .BindToLocalRotation(rtRectTransform))
                    .Run();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            _cardPlayController.OnDrag(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            _isDragging = false;
            bool played = _cardPlayController.EndDrag(eventData.position);
            if (!played) ResetDragVisual();
        }

        private void ResetDragVisual()
        {
            if (_hoverMotion.IsActive()) _hoverMotion.Cancel();
            {
                _hoverMotion = LSequence.Create()
                    .Append(LMotion.Create(rtRectTransform.localScale, Vector3.one , 0.11f)
                        .WithEase(Ease.OutQuad)
                        .BindToLocalScale(rtRectTransform))
                    .Join(LMotion.Create(rtRectTransform.rotation, Quaternion.identity, 0.11f)
                        .WithEase(Ease.OutQuad)
                        .BindToLocalRotation(rtRectTransform))
                    .Run();
            }
        }

        private void OnDestroy()
        {
            if (_hoverMotion.IsActive()) _hoverMotion.Cancel();
        }
    }
}
