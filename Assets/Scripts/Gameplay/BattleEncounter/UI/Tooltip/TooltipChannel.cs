using System;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
 
    [CreateAssetMenu(menuName = "Battle/TooltipChannel")]
    public class TooltipChannel : ScriptableObject
    {
        public event Action<TooltipData, Vector2> ShowRequested;
        public event Action HideRequested;

        public void Show(TooltipData data, Vector2 screenAnchor) => ShowRequested?.Invoke(data, screenAnchor);
        public void Hide() => HideRequested?.Invoke();
    }
}
