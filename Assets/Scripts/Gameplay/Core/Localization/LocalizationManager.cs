
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField] 
    private Button changeLanguageButton;
    private const string PrefKey = "lang";
    private TextMeshProUGUI _buttonText;

    private async void Start()
    {
        await LocalizationSettings.InitializationOperation.Task;
        string save = PlayerPrefs.GetString(PrefKey, "en");
        Apply(save);
        changeLanguageButton.onClick.AddListener(ToggleLanguage);
        _buttonText = changeLanguageButton.transform.GetComponentInChildren<TextMeshProUGUI>();
    }
    public void ToggleLanguage()
    {
        string cur = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code : "en";
        SetLanguage(cur == "th" ? "en" : "th");
    }
    public void SetLanguage(string code)
    {
        _buttonText.text = code;
        Apply(code);
        PlayerPrefs.SetString(PrefKey, code);
        PlayerPrefs.Save();
        
    }
    private void Apply(string code)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(code);
        if (locale != null) LocalizationSettings.SelectedLocale = locale;
    }

    private void OnDestroy()
    {
        changeLanguageButton.onClick.RemoveListener(ToggleLanguage);
    }
}
