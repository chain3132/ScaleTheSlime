using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
   
    public abstract class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] 
        protected TooltipChannel _channel;

        private bool _shown;

        protected abstract bool TryBuild(out TooltipData data);

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_channel == null) return;
            if (!TryBuild(out var data)) return;
            _channel.Show(data, ScreenAnchor(eventData));
            _shown = true;
        }

        public void OnPointerExit(PointerEventData eventData) => HideIfShown();

        protected virtual Vector2 ScreenAnchor(PointerEventData eventData) => eventData.position;

        private void HideIfShown()
        {
            if (!_shown) return;
            _shown = false;
            _channel?.Hide();
        }

        private void OnDisable() => HideIfShown();
    }
}
