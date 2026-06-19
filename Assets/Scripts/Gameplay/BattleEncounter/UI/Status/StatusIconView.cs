using Gameplay.BattleEncounter.Status;
using Gameplay.BattleEncounter.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconView : MonoBehaviour
{
    [SerializeField]
    Image _icon;
    [SerializeField]
    TMP_Text _amount;
    [SerializeField]
    StatusTooltipTrigger _tooltip;

    public void SetData(StatusType type, Sprite icon, int amount)
    {
        _icon.sprite = icon;
        _amount.text = amount.ToString();
        if (_tooltip != null) _tooltip.SetType(type);
    }

}
