using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

public class LanguageControl : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Sprite[] flags;
    [SerializeField] private Navigator navigator;
    
    public IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for(int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name) { image = flags.FirstOrDefault(x => x.name.Equals(locale.LocaleName, StringComparison.OrdinalIgnoreCase)) });
        }
        dropdown.options = options;
        dropdown.value = selected;
        dropdown.onValueChanged.AddListener(LocaleSelected);
    }

    private void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        navigator.NavigateToTitleScreen();
    }
}