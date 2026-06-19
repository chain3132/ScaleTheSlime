using System;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.UI.Card.Enum;
using Gameplay.BattleEncounter.UI.Characters;
using LitMotion;
using LitMotion.Extensions;
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
        private Enemy _targetEnemy;
        private EnemyView _hovered;
        private readonly Subject<CardPlay> _cardPlayed = new();

        #endregion

        public Observable<CardPlay> CardPlayed => _cardPlayed;
        public bool SelectionMode = false;
        public Subject<Data.Card> CardClicked = new();

        public bool Interactable { get; set; } = true;
        private BattleContext _context;
        public void SetContext(BattleContext context)
        {
            _context = context;
        }

        public void BeginDrag(Vector2 cardPosition,Data.Card card)
        {
            
            _currentCard = card;
            _targetEnemy = null;
            _origin = cardPosition;
            _arrowRT.gameObject.SetActive(true);
        }
        public void OnDrag(Vector2 mousePosition)
        {
            UpdateArrow(mousePosition, _origin);
            UpdateHoverEnemy(mousePosition);
            IfInPlayZone(mousePosition);
        }
        public bool EndDrag(Vector2 mousePosition)
        {
            _arrowRT.gameObject.SetActive(false);
            _playZoneUI.SetActive(false);
                ClearHover();   

            if (IsValidPlay(_currentCard, mousePosition))
            {
                _cardPlayed.OnNext(new CardPlay(_currentCard, _targetEnemy));
                _currentCard = null;
                _targetEnemy = null;
                return true;
            }
            _currentCard = null;
            _targetEnemy = null;
            return false;

        }
        private void UpdateHoverEnemy(Vector2 mousePosition)
        {
            EnemyView view = null;

            if (_currentCard != null && _currentCard.Definition.PlayTarget == CardTarget.Enemy)
                TryGetEnemyUnderMouse(mousePosition, out view);

            if (view == _hovered) return;
            if (_hovered != null)
            {
                _hovered.HighLight(false);
                _hovered.View.ClearDamagePreview();

            }    
            _hovered = view;
            if (_hovered != null) _hovered.HighLight(true);
            if (_hovered != null)
            {
                _hovered.View.ShowDamagePreview(PreviewUIFor(_currentCard,_hovered.Enemy));
            }
            
            
        }
        private int PreviewUIFor(Data.Card card, Enemy target)
        {
            if (_context == null) return 0;
            int total = 0;
            foreach (var e in card.Definition.Effects)
                if (e.Type == CardEffectType.Attack)
                    total += CardEffectExecutor.PreviewDisplay(e, _context, target) * Mathf.Max(1, e.Repeat);
            return total;
        }

        private void ClearHover()
        {
            if (_hovered != null)
            {
                _hovered.HighLight(false);
                _hovered.View.ClearDamagePreview();

            }
            _hovered = null;
        }
        private bool IsValidPlay(Data.Card card, Vector2 mousePosition)
        {
            if (card == null) return false;
            switch (card.Definition.PlayTarget)
            {
                case CardTarget.Player:
                case CardTarget.Background:
                case CardTarget.AllEnemies:
                    return RectTransformUtility.RectangleContainsScreenPoint(_playZoneRect, mousePosition, null);
                case CardTarget.Enemy:
                    return TryGetEnemyUnderMouse(mousePosition, out _targetEnemy);
                default:
                    return false; // None
            }
        }
        private bool TryGetEnemyUnderMouse(Vector2 screenPos, out EnemyView enemy)
        {
            enemy = null;
            Vector2 world = Camera.main.ScreenToWorldPoint(screenPos);
            Collider2D hit = Physics2D.OverlapPoint(world);
            if (hit != null && hit.TryGetComponent(out EnemyView view))
            {
                enemy = view;
                return true;
            }
            return false;
        }
        private bool TryGetEnemyUnderMouse(Vector2 screenPos, out Enemy enemy)
        {
            enemy = null;
            Vector2 world = Camera.main.ScreenToWorldPoint(screenPos);
            Collider2D hit = Physics2D.OverlapPoint(world);
            if (hit != null && hit.TryGetComponent(out EnemyView view))
            {
                enemy = view.Enemy;
                return true;
            }
            return false;
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
                case CardTarget.AllEnemies: 
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
            CardClicked.Dispose();
        }
    }
}
