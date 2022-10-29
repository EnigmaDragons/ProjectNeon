using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName="OnlyOnce/ToggleChineseDevTool")]
public class LocalizationToggleChinese : ScriptableObject
{
    public void ToggleChinese()
    {
        var before = LocalizationManager.CurrentLanguage;
        var newLanguageName = before == "Chinese" ? "English" : "Chinese";
        var newLanguageCode = before == "Chinese" ? "en" : "zh";
        LocalizationManager.SetLanguageAndCode(newLanguageName, newLanguageCode);
        Message.Publish(new CheatAcceptedSuccessfully());
    }
}
