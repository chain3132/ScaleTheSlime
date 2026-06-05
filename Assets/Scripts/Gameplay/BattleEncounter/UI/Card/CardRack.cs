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
        [SerializeField] 
        private string _cardPrefabAddress = "CardPrefab";
        [SerializeField]
        private SplineContainer _drawSpline;
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

            var card = Instantiate(_cardPrefab, transform).GetComponent<CardView>();
            card.Bind(vm);
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
