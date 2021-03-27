using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UIBodyColor : MonoBehaviour
    {
        /// <summary>
        /// Image preview of the color.
        /// </summary>
        [Tooltip("Image preview of the color")]
        public Image colorImg;
        
        private bool _uicolorenabled;
        private UICreator _uicreator;

        void Awake()
        {
            _uicreator = this.transform.GetComponentInParent<UICreator>();
        }

        void OnEnable()
        {
            setUIColorEnable(false);
        }

        /// <summary>
        /// Request to open UIColor and edit current color of interest.
        /// </summary>
        public void EditColor()
        {
            setUIColorEnable(true);
        }

        /// <summary>
        /// Adjust color's opacity.
        /// </summary>
        /// <param name="opacity">Opacity value</param>
        public void SetOpacity(float opacity)
        {
            Color detailscolor = _uicreator.character.GetPartColor(SlotCategory.SkinDetails, ColorCode.Color1);
            detailscolor.a = Mathf.Clamp01(opacity);
            _uicreator.character.SetPartColor(SlotCategory.SkinDetails, ColorCode.Color1, detailscolor);
        }

        public void CopyColor()
        {
            Clipboard.color = colorImg.color;
        }

        public void PasteColor()
        {
            if (Clipboard.color == Color.clear || _uicreator == null)
                return;

            _uicreator.character.SkinColor = Clipboard.color;
            colorImg.color = Clipboard.color;
        }

        private void setUIColorEnable(bool enableUIColor)
        {
            if (_uicreator == null)
                return;

            _uicolorenabled = enableUIColor;
            if (_uicolorenabled)
            {
                _uicreator.colorUI.Show(_uicreator.character.SkinColor);
                setChildActive(false);
            }
            else
            {
                colorImg.color = _uicreator.character.SkinColor;
                _uicreator.colorUI.Close();
                setChildActive(true);
            }
        }

        void Update()
        {
            if (!_uicolorenabled)
                return;

            if (!_uicreator.colorUI.gameObject.activeInHierarchy)
            {
                setUIColorEnable(false);
            }
            else
            {
                _uicreator.character.SkinColor = _uicreator.colorUI.selectedColor;
            }
        }

        private void setChildActive(bool isActive)
        {
            for (int i = 0; i < this.transform.childCount; i++)
                this.transform.GetChild(i).gameObject.SetActive(isActive);
        }
    }
}