using System.Collections.Generic;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.Characters.Enums;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
 
    public class PassiveTooltipTrigger : TooltipTrigger
    {
        [SerializeField] 
        private SizeFormInfoDatabase _database;
        [SerializeField] 
        private string _title = "PASSIVE";

        private static readonly SizeForm[] Forms = { SizeForm.Tiny, SizeForm.Normal, SizeForm.Giant };

        private IPassiveProvider _provider;

        public void SetProvider(IPassiveProvider provider) => _provider = provider;

        protected override bool TryBuild(out TooltipData data)
        {
            data = default;
            if (_provider == null) return false;

            var lines = new List<TooltipLine>();
            foreach (var form in Forms)
            {
                var passive = _provider.PassiveFor(form);
                if (passive == null || string.IsNullOrEmpty(passive.Description)) continue;

                string badge = _database != null ? _database.BadgeFor(form) : string.Empty;
                Color color = _database != null ? _database.ColorFor(form) : Color.white;
                lines.Add(new TooltipLine(badge, color, passive.Description));
            }

            if (lines.Count == 0) return false;
            data = TooltipData.Passive(_title, lines);
            return true;
        }
    }
}
