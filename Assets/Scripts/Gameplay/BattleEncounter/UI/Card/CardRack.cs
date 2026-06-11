using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Splines;

namespace Gameplay.BattleEncounter.UI.Card
{
    public class CardRack : MonoBehaviour
    {
        #region References

        [SerializeField] 
        private string _cardPrefabAddress = "CardPrefab";
        [SerializeField]
        private SplineContainer _drawSpline;
        [SerializeField]
        private CardPlayController _cardPlayController;
        [SerializeField]
        private RectTransform _cardContainer;
        [SerializeField]
        private RectTransform _discardPilePoint;   

        #endregion

        #region Setup

        [SerializeField]
        private float _drawDuration = 0.4f;
        [SerializeField]
        private float _arrangeDuration = 0.2f;
        [SerializeField, 
         Range(0f, 1f)]
        private float _centerT = 0.5f;   
        [SerializeField]
        private float _maxStep = 0.15f;  
        [SerializeField,
         Range(0f, 1f)]
        private float _maxSpread = 0.9f; 
        [SerializeField]
        private float _stagger = 0.08f;
        [SerializeField]
        private float _showcaseDuration = 0.25f;   
        [SerializeField]
        private float _toDiscardDuration = 0.3f;   

        #endregion
        

        private readonly List<CardView> _hand = new();
        private AsyncOperationHandle<GameObject> _cardPrefabHandle;
        private GameObject _cardPrefab;

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            if (_cardPrefab != null) return; 
            _cardPrefabHandle = Addressables.LoadAssetAsync<GameObject>(_cardPrefabAddress);
            _cardPrefab = await _cardPrefabHandle.ToUniTask(cancellationToken: ct);
        }

        private void OnDestroy()
        {
            if (_cardPrefabHandle.IsValid())
                Addressables.Release(_cardPrefabHandle);
        }

        public async UniTask DrawAsync(CardViewModel vm, CancellationToken ct)
        {
            if (_cardPrefab == null) return; 

            var card = Instantiate(_cardPrefab, _cardContainer).GetComponent<CardView>();
            card.Bind(vm,_cardPlayController);
            _hand.Add(card);

            int n = _hand.Count;
            float step = StepFor(n);

            for (int i = 0; i < n - 1; i++)
                TweenToSlot(_hand[i], SlotT(i, n, step));

            float slotT = SlotT(n - 1, n, step);
            var rt = (RectTransform)card.transform;
            await LMotion.Create(0f, slotT, _drawDuration)
                .WithEase(Ease.OutCubic)
                .Bind(p => rt.anchoredPosition = SplineToAnchored(p))
                .ToUniTask(ct);
        }

        public async UniTask PlayCardAsync(Data.Card card, CancellationToken ct)
        {
            var view = _hand.Find(c => c.Card == card);
            if (view == null) return;

            _hand.Remove(view);
            Arrange();                       
            var rt = (RectTransform)view.transform;
            rt.SetAsLastSibling();          
            var parent = (RectTransform)rt.parent;

            Vector2 ToLocal(Vector2 screen)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screen, null, out var p);
                return p;
            }

            
            Vector2 center = ToLocal(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
            await UniTask.WhenAll(
                LMotion.Create(rt.anchoredPosition, center, _showcaseDuration)
                    .WithEase(Ease.OutCubic).BindToAnchoredPosition(rt).ToUniTask(ct),
                LMotion.Create(rt.localScale, Vector3.one * 1.2f, _showcaseDuration)
                    .WithEase(Ease.OutCubic).BindToLocalScale(rt).ToUniTask(ct));

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f), cancellationToken: ct);

            Vector2 discard = ToLocal(RectTransformUtility.WorldToScreenPoint(null, _discardPilePoint.position));
            await UniTask.WhenAll(
                LMotion.Create(rt.anchoredPosition, discard, _toDiscardDuration)
                    .WithEase(Ease.InCubic).BindToAnchoredPosition(rt).ToUniTask(ct),
                LMotion.Create(rt.localScale, Vector3.one * 0.2f, _toDiscardDuration)
                    .WithEase(Ease.InCubic).BindToLocalScale(rt).ToUniTask(ct));

            
            Destroy(view.gameObject);
            // waiting for change this to object pooling 
        }

        
        private void Arrange()
        {
            int n = _hand.Count;
            float step = StepFor(n);
            for (int i = 0; i < n; i++)
                TweenToSlot(_hand[i], SlotT(i, n, step));
        }

        public async UniTask DiscardHandAsync(System.Func<Data.Card, bool> keep, CancellationToken ct)
        {
            var toDiscard = _hand.FindAll(v => keep == null || !keep(v.Card));
            if (toDiscard.Count == 0) return;

            foreach (var v in toDiscard) _hand.Remove(v);
            Arrange();   

            var tasks = new List<UniTask>(toDiscard.Count);
            foreach (var v in toDiscard) tasks.Add(ToDiscardAsync(v, ct));
            await UniTask.WhenAll(tasks);
        }

        private async UniTask ToDiscardAsync(CardView view, CancellationToken ct)
        {
            var rt = (RectTransform)view.transform;
            var parent = (RectTransform)rt.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent, RectTransformUtility.WorldToScreenPoint(null, _discardPilePoint.position), null, out Vector2 discard);

            await UniTask.WhenAll(
                LMotion.Create(rt.anchoredPosition, discard, _toDiscardDuration)
                    .WithEase(Ease.InCubic).BindToAnchoredPosition(rt).ToUniTask(ct),
                LMotion.Create(rt.localScale, Vector3.one * 0.2f, _toDiscardDuration)
                    .WithEase(Ease.InCubic).BindToLocalScale(rt).ToUniTask(ct));

            Destroy(view.gameObject);
        }

        public async UniTask DrawManyAsync(IReadOnlyList<CardViewModel> vms, CancellationToken ct)
        {
            foreach (var vm in vms)
            {
                await DrawAsync(vm, ct);
                await UniTask.Delay(System.TimeSpan.FromSeconds(_stagger), cancellationToken: ct);
            }
        }

        private float StepFor(int n) =>
            n <= 1 ? 0f : Mathf.Min(_maxStep, _maxSpread / (n - 1));

        private float SlotT(int i, int n, float step) =>
            Mathf.Clamp01(_centerT + ((n - 1) * 0.5f - i) * step);

        private void TweenToSlot(CardView card, float t)
        {
            var rt = (RectTransform)card.transform;
            LMotion.Create(rt.anchoredPosition, SplineToAnchored(t), _arrangeDuration)
                .WithEase(Ease.OutCubic)
                .BindToAnchoredPosition(rt);
        }

        private Vector2 SplineToAnchored(float t)
        {
            Vector3 world = _drawSpline.EvaluatePosition(t);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, world);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform, screen, null, out Vector2 local);
            return local;
        }
    }
}
