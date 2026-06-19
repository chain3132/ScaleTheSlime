using Gameplay.BattleEncounter.Status;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public class StatusTooltipTrigger : TooltipTrigger
    {
        [SerializeField] 
        private StatusDatabase _database;

        private StatusType _type;

        public void SetType(StatusType type) => _type = type;

        protected override bool TryBuild(out TooltipData data)
        {
            string title = _database != null ? _database.NameFor(_type) : _type.ToString();
            string body = _database != null ? _database.DescriptionFor(_type) : string.Empty;
            data = TooltipData.Standard(title, body);
            return true;
        }
    }
}
