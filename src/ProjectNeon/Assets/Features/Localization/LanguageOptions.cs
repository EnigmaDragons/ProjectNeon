using System;
using I2.Loc;
using TMPro;
using UnityEngine;

[Serializable]
public class LanguageOption
{
    public string OptionText;
    public TMP_FontAsset OptionFont;
    public Sprite FlagSprite;
    public string Language;
    public string LanguageCode;

    public void Select()
    {
        LocalizationManager.SetLanguageAndCode(Language, LanguageCode);
        CurrentAcademyData.Mutate(a => a.HasSelectedLanguage = true);
    }
}
