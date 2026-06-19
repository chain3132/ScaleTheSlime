using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public enum TooltipStyle { Standard, Passive }

    public readonly struct TooltipLine
    {
        public readonly string Badge;
        public readonly Color BadgeColor;
        public readonly string Body;

        public TooltipLine(string badge, Color badgeColor, string body)
        {
            Badge = badge;
            BadgeColor = badgeColor;
            Body = body;
        }
    }


    public readonly struct TooltipData
    {
        public readonly TooltipStyle Style;
        public readonly string Title;
        public readonly string Body;
        public readonly IReadOnlyList<TooltipLine> Lines;

        private TooltipData(TooltipStyle style, string title, string body, IReadOnlyList<TooltipLine> lines)
        {
            Style = style;
            Title = title;
            Body = body;
            Lines = lines;
        }

        public static TooltipData Standard(string title, string body)
            => new(TooltipStyle.Standard, title, body, null);

        public static TooltipData Passive(string title, IReadOnlyList<TooltipLine> lines)
            => new(TooltipStyle.Passive, title, null, lines);
    }
}
