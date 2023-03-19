using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc
{
    [RequireComponent(typeof(TMP_Dropdown))]
    [AddComponentMenu("I2/Localization/SetLanguage Dropdown TMP")]
    public class SetLanguageDropdownTmp : MonoBehaviour
    {
        private TMP_Dropdown _d;
        
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER
        void Awake()
        {
            try
            {
                var dropdown = GetComponent<TMP_Dropdown>();
                if (dropdown == null)
                    return;

                _d = dropdown;
                var currentLanguage = LocalizationManager.CurrentLanguage;
                if (LocalizationManager.Sources.Count == 0) LocalizationManager.UpdateSources();
                var languages = LocalizationManager.GetAllLanguages();

                // Fill the dropdown elements
                dropdown.ClearOptions();
                dropdown.AddOptions(languages);

                dropdown.value = languages.IndexOf(currentLanguage);
                dropdown.onValueChanged.RemoveListener(OnValueChanged);
                dropdown.onValueChanged.AddListener(OnValueChanged);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

		
        void OnValueChanged( int index )
        {
            if (index<0)
            {
                index = 0;
                _d.value = index;
            }
            
            LocalizationManager.CurrentLanguage = _d.options[index].text;
        }
#endif
    }
}