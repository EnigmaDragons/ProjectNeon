using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UIColor : MonoBehaviour
    {
        /// <summary>
        /// Current mode of this UIColor. Tells whether UIColor is opening color palette or color picker.
        /// </summary>
        [Tooltip("Current mode of this UIColor. Tells whether UIColor is opening color palette or color picker")]
        [ReadOnly]
        public UIColorMode mode;

        [ReadOnly]
        public Color selectedColor;

        /// <summary>
        /// UIColorPalette managed by this UIColor.
        /// </summary>
        [Tooltip("UIColorPalette managed by this UIColor")]
        public UIColorPalette colorPalette;

        /// <summary>
        /// UIColorPicker managed by this UIColor.
        /// </summary>
        [Tooltip("UIColorPicker managed by this UIColor")]
        public UIColorPicker colorPicker;

        /// <summary>
        /// Scrollbar controlling color palette's contents
        /// </summary>
        [Tooltip("Scrollbar controlling color palette's contents")]
        public Transform scrollBar;

        /// <summary>
        /// Show this UIColor.
        /// </summary>
        public void Show()
        {
            Show(this.selectedColor);
        }

        /// <summary>
        /// Show this UIColor.
        /// </summary>
        /// <param name="currentColor">Initiated color to be showed.</param>
        public void Show(Color currentColor)
        {
            selectedColor = currentColor;
            colorPalette.color = this.selectedColor;
            colorPicker.color = this.selectedColor;
            setMode(this.mode);
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Close this UIColor.
        /// </summary>
        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Show color palette and close color picker.
        /// </summary>
        public void ShowColorPalette()
        {
            setMode(UIColorMode.Palette);
        }

        /// <summary>
        /// Show color picker and close color palette.
        /// </summary>
        public void ShowColorPicker()
        {
            setMode(UIColorMode.Picker);
        }

        private void setMode(UIColorMode colorMode)
        {
            this.mode = colorMode;
            switch (this.mode)
            {
                case UIColorMode.Palette:
                    this.colorPalette.gameObject.SetActive(true);
                    this.colorPicker.gameObject.SetActive(false);
                    this.scrollBar.gameObject.SetActive(true);
                    break;
                case UIColorMode.Picker:
                    this.colorPalette.gameObject.SetActive(false);
                    this.colorPicker.gameObject.SetActive(true);
                    this.scrollBar.gameObject.SetActive(false);
                    break;
                default:
                    this.colorPalette.gameObject.SetActive(false);
                    this.colorPicker.gameObject.SetActive(false);
                    this.scrollBar.gameObject.SetActive(false);
                    break;
            }
        }

        void Update()
        {
            switch (mode)
            {
                case UIColorMode.Palette:
                    this.selectedColor = colorPalette.color;
                    colorPicker.color = colorPalette.color;
                    break;
                case UIColorMode.Picker:
                    this.selectedColor = colorPicker.color;
                    colorPalette.color = colorPicker.color;
                    break;
                default:
                    break;
            }
        }
    }

    public static class Clipboard
    {
        public static Color color = Color.clear;
    }
}