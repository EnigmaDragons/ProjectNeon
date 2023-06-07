using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InputIcons
{
    public class InputIconsText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textMeshProUGUI;

        [SerializeField]
        private TextMeshPro textMeshPro;

        public void SetDirty()
        {
            if (textMeshProUGUI)
                textMeshProUGUI.SetAllDirty();

            if(textMeshPro)
                textMeshPro.SetAllDirty();
        }

        private void Reset()
        {
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            textMeshPro = GetComponent<TextMeshPro>();
        }

        private void OnEnable()
        {
            //Register with the manager to update this text object when the user changes devices
            InputIconsManagerSO.RegisterInputIconsText(this);
        }

        private void OnDisable()
        {
            //Unregister when not needed
            InputIconsManagerSO.UnregisterInputIconsText(this);
        }
    }
}
