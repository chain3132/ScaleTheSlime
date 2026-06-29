using Gameplay.BattleEncounter.Characters.Enums;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public class SizeFormTooltipTrigger : TooltipTrigger
    {
        [SerializeField] 
        private SizeFormInfoDatabase _database;
        [SerializeField] 
        private SizeForm _form = SizeForm.Tiny;

        protected override bool TryBuild(out TooltipData data)
        {
            var info = _database != null ? _database.InfoFor(_form) : null;
            if (info == null) { data = default; return false; }
            data = TooltipData.Standard(info.Title.GetLocalizedString(), info.Description.GetLocalizedString());
            return true;
        }
    }
}
