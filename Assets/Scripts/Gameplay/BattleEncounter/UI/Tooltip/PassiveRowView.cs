using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public class PassiveRowView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _badge;
        [SerializeField]
        private TMP_Text _body;

        public void Bind(TooltipLine line)
        {
            if (_badge != null) _badge.text = line.Badge;
            if (_badge != null) _badge.color = line.BadgeColor;
            if (_body != null) _body.text = line.Body;
        }
    }
}
