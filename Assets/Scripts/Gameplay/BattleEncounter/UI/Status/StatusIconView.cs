using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconView : MonoBehaviour
{
    [SerializeField] 
    Image _icon;
    [SerializeField] 
    TMP_Text _amount;
    
    public void SetData(Sprite icon, int amount)
    {
        _icon.sprite = icon;
        _amount.text = amount.ToString();
    }
    
}
