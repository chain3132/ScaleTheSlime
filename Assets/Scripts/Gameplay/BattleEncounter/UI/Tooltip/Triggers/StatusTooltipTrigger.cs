using Gameplay.BattleEncounter.Status;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public class StatusTooltipTrigger : TooltipTrigger
    {
        [SerializeField] 
        private StatusDatabase _database;

        private StatusType _type;
        private int _value;

        public void SetData(StatusType type, int value)
        {
            _type = type;
            _value = value;
        }

        protected override bool TryBuild(out TooltipData data)
        {
            string title = _database != null ? _database.NameFor(_type) : _type.ToString();
            string body = _database != null ? _database.DescriptionFor(_type) : string.Empty;
            if (!string.IsNullOrEmpty(body))
                body = body.Replace("{value}", _value.ToString());
            data = TooltipData.Standard(title, body);
            return true;
        }
    }
}
