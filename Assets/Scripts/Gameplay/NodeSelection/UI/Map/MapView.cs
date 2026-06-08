using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.NodeSelection.UI.Nodes;
using LitMotion;
using LitMotion.Extensions;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.NodeSelection.UI.Map
{
    public class MapView : MonoBehaviour
    {
        #region References

        [Header("Scene references")]
        [SerializeField] 
        private RectTransform _content;    
        [SerializeField] 
        private RectTransform _lineLayer; 
        [SerializeField] 
        private RectTransform _nodeLayer; 
        [SerializeField] 
        private RectTransform _player; 

        #endregion

        #region MapSetup

        [Header("Layout")]
        [SerializeField] 
        private float _xSpacing ;
        [SerializeField] 
        private float _ySpacing ;
        [SerializeField] 
        private float _xPadding ;
        [SerializeField] 
        private float _lineThickness = 6f;
        [SerializeField] 
        private float _moveSpeed ;

        #endregion
        [SerializeField]
        private SerializableMotionSettings<Vector2,NoOptions> _playerMoveSettings;


        #region fields

        private GameObject _nodePrefab;
        private NodeDatabase _database;
        private MapViewModel _vm;
        private readonly Dictionary<int, Vector2> _positions = new();
        private readonly List<GameObject> _spawned = new();
        private readonly CompositeDisposable _bindings = new();
        private CancellationTokenSource _moveCts;
        private MotionHandle _moveMotion;

        #endregion
 
        
        
        // Called by the entry point
        public void Initialize(MapViewModel vm,
            GameObject nodePrefab,
            NodeDatabase database)
        {
            _vm = vm;
            _nodePrefab = nodePrefab;
            _database = database;
            Build();
        }
 
        private void Build()
        {
            
            StretchToFill(_lineLayer);
            ComputePositions();
            ResizeContent();
            DrawLines();
            SpawnNodes();
            PlacePlayerAtCurrent();
        }
 
        private void ComputePositions()
        {
            _positions.Clear();
 
            var perLayer = new Dictionary<int, int>();
            foreach (var n in _vm.Nodes)
                perLayer[n.Layer] = perLayer.TryGetValue(n.Layer, out var c) ? c + 1 : 1;
 
            foreach (var n in _vm.Nodes)
            {
                int count = perLayer[n.Layer];
                float x = _xPadding + n.Layer * _xSpacing;
                float y = (n.IndexInLayer - (count - 1) * 0.5f) * _ySpacing;
                _positions[n.Id] = new Vector2(x, y);
            }
        }
 
        private void ResizeContent()
        {
            int maxLayer = 0;
            foreach (var n in _vm.Nodes) if (n.Layer > maxLayer) maxLayer = n.Layer;
            float width = _xPadding * 2 + maxLayer * _xSpacing;
            _content.sizeDelta = new Vector2(width, _content.sizeDelta.y);
        }
 
        private void DrawLines()
        {
            foreach (var edge in _vm.Edges)
                CreateLine(_positions[edge.FromId], _positions[edge.ToId]);
        }
 
        private void CreateLine(Vector2 a, Vector2 b)
        {
            var go = new GameObject("Line", typeof(Image));
            go.transform.SetParent(_lineLayer, false);
 
            var rt = (RectTransform)go.transform;
            AnchorLeftMiddle(rt);
            Vector2 dir = b - a;
            rt.sizeDelta = new Vector2(dir.magnitude, _lineThickness);
            rt.anchoredPosition = a + dir * 0.5f;
            rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
 
            go.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
            _spawned.Add(go);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SpawnNodes()
        {
            foreach (var nodeVm in _vm.Nodes)
            {
                var go = Instantiate(_nodePrefab, _nodeLayer);
                var rt = (RectTransform)go.transform;
                rt.anchoredPosition = _positions[nodeVm.Id];
                go.GetComponent<NodeView>().Bind(nodeVm,_database.SpriteFor(nodeVm.Type));
                _spawned.Add(go);
            }
        }
 
        private void PlacePlayerAtCurrent()
        {
            foreach (var n in _vm.Nodes)
            {
                if (n.IsCurrent.CurrentValue)
                {
                    _player.anchoredPosition = _positions[n.Id];
                    return;
                }
            }
        }
        
        public UniTask MoveTokenToAsync(int nodeId, CancellationToken ct)
        {
            return MoveAsync(nodeId, ct);
        }

        private UniTask MoveAsync(int nodeId, CancellationToken ct)
        {
            if (_moveMotion.IsActive()) _moveMotion.Cancel();

            var settings = _playerMoveSettings with
            {
                StartValue = _player.anchoredPosition,
                EndValue   = _positions[nodeId],
            };

            _moveMotion = LMotion.Create(settings).BindToAnchoredPosition(_player);
            return _moveMotion.ToUniTask(ct);
        }
 
        private void StretchToFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        
        private void AnchorLeftMiddle(RectTransform rt)
        {
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
        }
 
        public void Teardown()
        {
            if (_moveMotion.IsActive()) {_moveMotion.Cancel();} 
            _bindings.Clear();
            foreach (var go in _spawned) Destroy(go);
            _spawned.Clear();
            _positions.Clear();
        }
    }
}
