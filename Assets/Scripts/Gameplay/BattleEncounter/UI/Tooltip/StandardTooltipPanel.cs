using TMPro;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public class StandardTooltipPanel : TooltipPanel
    {
        [SerializeField] 
        private TMP_Text _title;
        [SerializeField]
        private TMP_Text _body;

        public override void Bind(TooltipData data)
        {
            if (_title != null)
            {
                _title.text = data.Title;
                _title.gameObject.SetActive(!string.IsNullOrEmpty(data.Title));
            }
            if (_body != null) _body.text = data.Body;
        }
    }
}
