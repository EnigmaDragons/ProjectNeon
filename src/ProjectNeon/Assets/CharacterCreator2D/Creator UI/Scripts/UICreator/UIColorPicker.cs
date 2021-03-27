using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UIColorPicker : MonoBehaviour
    {
        /// <summary>
        /// Current selected color.
        /// </summary>
        [Tooltip("Current selected color")]
        [ReadOnly]
        public Color color = Color.gray;

        /// <summary>
        /// Image preview of current selected color.
        /// </summary>
        [Tooltip("Image preview of current selected color")]
        public Image colorPreview;

        /// <summary>
        /// UI Slider used to adjust the hue value of the current selected color.
        /// </summary>
        [Tooltip("UI Slider used to adjust the hue value of current selected color")]
        public Slider hueSlider;

        /// <summary>
        /// Transform object of the palette's image.
        /// </summary>
        [Tooltip("Transform object of the palette's image")]
        public RectTransform palette;

        /// <summary>
        /// Transform object used for highlighting current selected color on the palette.
        /// </summary>
        [Tooltip("Transform object used for highlighting current selected color on the palette")]
        public RectTransform picker;

        /// <summary>
        /// The material used in this UIColorPicker.
        /// </summary>
        [Tooltip("The material used in this UIColorPicker")]
        public Material colorPickerMaterial;

        /// <summary>
        /// The Input Field used for hex code color.
        /// </summary>
        [Tooltip("The material used in this UIColorPicker")]
        public InputField hexInputField;

        float hue;
        float saturation;
        float brightness;
        Vector2 pickerPos;
        bool hexInput;

        void OnEnable()
        {
            SetPos();
            SetColor();
        }

        /// <summary>
        /// Pick a color according to pointer's potition over palette.
        /// </summary>
        public void PickColor()
        {
            picker.position = Input.mousePosition;
            pickerPos.Set(Mathf.Clamp(picker.anchoredPosition.x, 0, palette.sizeDelta.x),
                Mathf.Clamp(picker.anchoredPosition.y, 0, palette.sizeDelta.y));
            picker.anchoredPosition = pickerPos;
            SetColor();
        }

        /// <summary>
        /// Pick a color using hex code.
        /// </summary>
        public void InputColor(string hex)
        {
            hexInput = true;

            if (!hex.Contains("#"))
                hex = "#" + hex;

            Color hexColor;

            if (ColorUtility.TryParseHtmlString(hex, out hexColor))
            {
                color = hexColor;
                colorPreview.color = color;
                SetPos();
            }

            hexInputField.text = "#" + ColorUtility.ToHtmlStringRGB(color);
            
            hexInput = false;
        }

        /// <summary>
        /// Assign this color's value according to picker's position and hueSlider's value.
        /// </summary>
        public void SetColor()
        {
            hue = hueSlider.value;
            colorPickerMaterial.SetFloat("_Hue", hue);
            saturation = picker.anchoredPosition.x / palette.sizeDelta.x;
            brightness = picker.anchoredPosition.y / palette.sizeDelta.y;
            color = Color.HSVToRGB(hue, saturation, brightness);
            colorPreview.color = color;
            if (!hexInput)
                hexInputField.text = "#" + ColorUtility.ToHtmlStringRGB(color);
        }

        /// <summary>
        /// Set this picker's position according to color's value.
        /// </summary>
        public void SetPos()
        {
            Color.RGBToHSV(color, out hue, out saturation, out brightness);
            pickerPos.Set(saturation * palette.sizeDelta.x, brightness * palette.sizeDelta.y);
            picker.anchoredPosition = pickerPos;
            hueSlider.value = hue;
        }

        /// <summary>
        /// Add current color's value to customPalette of a UIColor object found in this Transform's parent.
        /// </summary>
        public void AddCurrentToPalette()
        {
            UIColor uicolor = this.transform.GetComponentInParent<UIColor>();
            if (uicolor == null)
                return;

            uicolor.colorPalette.AddToCustomPalette(this.color);
        }
    }
}