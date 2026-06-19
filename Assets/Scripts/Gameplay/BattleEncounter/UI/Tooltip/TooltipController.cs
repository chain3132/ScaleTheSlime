using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] 
        private TooltipChannel _channel;
        [SerializeField] 
        private Canvas _canvas;
        [SerializeField] 
        private StandardTooltipPanel _standardPanel;
        [SerializeField] 
        private PassiveTooltipPanel _passivePanel;
        [SerializeField] 
        private Vector2 _offset = new(0f, 16f);

        private TooltipPanel _active;

        private void Awake()
        {
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
        }

        private void OnEnable()
        {
            HideAll();
            if (_channel == null) return;
            _channel.ShowRequested += Show;
            _channel.HideRequested += Hide;
        }

        private void OnDisable()
        {
            if (_channel == null) return;
            _channel.ShowRequested -= Show;
            _channel.HideRequested -= Hide;
        }

        private void Show(TooltipData data, Vector2 screenAnchor)
        {
            HideAll();
            _active = data.Style == TooltipStyle.Passive ? (TooltipPanel)_passivePanel : _standardPanel;
            if (_active == null) return;

            _active.Bind(data);
            _active.gameObject.SetActive(true);
            // ContentSizeFitter needs a forced rebuild before we can read the size.
            LayoutRebuilder.ForceRebuildLayoutImmediate(_active.Rect);
            Position(_active.Rect, screenAnchor);
        }

        private void Hide()
        {
            if (_active != null) _active.gameObject.SetActive(false);
            _active = null;
        }

        private void HideAll()
        {
            if (_standardPanel != null) _standardPanel.gameObject.SetActive(false);
            if (_passivePanel != null) _passivePanel.gameObject.SetActive(false);
            _active = null;
        }

        private void Position(RectTransform tip, Vector2 screenAnchor)
        {
            if (_canvas == null) return;
            var canvasRect = (RectTransform)_canvas.transform;
            var cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenAnchor, cam, out var local);
            local += _offset;
            tip.localPosition = Clamp(tip, canvasRect, local);
        }

        // Keep the whole tooltip rect inside the canvas, accounting for its pivot.
        private static Vector2 Clamp(RectTransform tip, RectTransform canvas, Vector2 pos)
        {
            Vector2 size = tip.rect.size;
            Vector2 pivot = tip.pivot;
            Rect cr = canvas.rect;
            pos.x = Mathf.Clamp(pos.x, cr.xMin + size.x * pivot.x, cr.xMax - size.x * (1f - pivot.x));
            pos.y = Mathf.Clamp(pos.y, cr.yMin + size.y * pivot.y, cr.yMax - size.y * (1f - pivot.y));
            return pos;
        }
    }
}
