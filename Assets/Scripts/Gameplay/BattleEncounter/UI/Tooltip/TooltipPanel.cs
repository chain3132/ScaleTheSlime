using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public abstract class TooltipPanel : MonoBehaviour
    {
        [SerializeField] 
        private RectTransform _rect;

        public RectTransform Rect => _rect != null ? _rect : (RectTransform)transform;

        public abstract void Bind(TooltipData data);
    }
}
