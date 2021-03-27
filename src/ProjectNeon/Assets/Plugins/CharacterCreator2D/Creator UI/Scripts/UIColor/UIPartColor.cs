using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UIPartColor : MonoBehaviour
    {
        /// <summary>
        /// Current state of UIPartColor.
        /// </summary>
        [Tooltip("Current state of UIPartColor")]
        [ReadOnly]
        public UIPartColorState state;

        /// <summary>
        /// Current mode of UIPartColor.
        /// </summary>
        [Tooltip("Current mode of UIPartColor")]
        [ReadOnly]
        public UIPartColorMode mode;

        /// <summary>
        /// SlotCategory displayed by this UIPartColor.
        /// </summary>
        [Tooltip("SlotCategory displayed by this UIPartColor")]
        [ReadOnly]
        public SlotCategory slotCategory;

        /// <summary>
        /// Image preview of the color of a part
        /// </summary>
        [Tooltip("Image preview of the first color of a part")]
        public Image color1Img;

        /// <summary>
        /// Image preview of the second color of a part
        /// </summary>
        [Tooltip("Image preview of the second color of a part")]
        public Image color2Img;

        /// <summary>
        /// Image preview of the third color of a part
        /// </summary>
        [Tooltip("Image preview of the third color of a part")]
        public Image color3Img;

        /// <summary>
        /// Slider that adjust color's opacity.
        /// </summary>
        [Tooltip("Slider that adjust color's opacity")]
        public Slider colorSlider;

        /// <summary>
        /// Text message that tells if a part can't be colored.
        /// </summary>
        [Tooltip("Text message that tells if a part can't be colored")]
        public Text uncolorableText;

        public GameObject hairColor;
        public GameObject skinColor;

        private UICreator _uicreator;
        private string _colorcode;

        void Awake()
        {
            _uicreator = this.transform.GetComponentInParent<UICreator>();
        }

        void OnEnable()
        {
            setState(UIPartColorState.None);
        }
        
        /// <summary>
        /// Initialize UIPartColor according to SlotCategory value.
        /// </summary>
        /// <param name="slotCategory">SlotCategory value.</param>
        public void Initialize(SlotCategory slotCategory)
        {
            if (_uicreator == null)
                _uicreator = this.transform.GetComponentInParent<UICreator>();
            this.slotCategory = slotCategory;
            if (_uicreator.character.slots.GetSlot(slotCategory).material == null)
                this.mode = UIPartColorMode.Uncolorable;
            else if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory) || slotCategory == SlotCategory.SkinDetails)
                this.mode = UIPartColorMode.OneColor;
            else
                this.mode = UIPartColorMode.ThreeColor;
        }

        /// <summary>
        /// Request to open UIColor and edit the first color of selected part.
        /// </summary>
        public void EditColor1()
        {
            setState(UIPartColorState.Color1);
        }

        /// <summary>
        /// Request to open UIColor and edit the second color of selected part.
        /// </summary>
        public void EditColor2()
        {
            setState(UIPartColorState.Color2);
        }

        /// <summary>
        /// Request to open UIColor and edit the third color of selected part.
        /// </summary>
        public void EditColor3()
        {
            setState(UIPartColorState.Color3);
        }

        /// <summary>
        /// Adjust color's opacity.
        /// </summary>
        /// <param name="opacity">Opacity value</param>
        public void SetOpacity(float opacity)
        {
            switch (slotCategory)
            {
                case SlotCategory.SkinDetails:
                    Color detailscolor = _uicreator.character.GetPartColor(SlotCategory.SkinDetails, ColorCode.Color1);
                    detailscolor.a = Mathf.Clamp01(opacity);
                    _uicreator.character.SetPartColor(SlotCategory.SkinDetails, ColorCode.Color1, detailscolor);
                    break;
                default:
                    break;
            }
        }

        public void CopyColor (int col)
        {
            if (col == 1) Clipboard.color = color1Img.color;
            if (col == 2) Clipboard.color = color2Img.color;
            if (col == 3) Clipboard.color = color3Img.color;
        }
        public void PasteColor (int col)
        {
            if (Clipboard.color == Color.clear || _uicreator == null) return;
            if (col == 1) _uicreator.character.SetPartColor(slotCategory, ColorCode.Color1, Clipboard.color);
            if (col == 2) _uicreator.character.SetPartColor(slotCategory, ColorCode.Color2, Clipboard.color);
            if (col == 3) _uicreator.character.SetPartColor(slotCategory, ColorCode.Color3, Clipboard.color);
            refreshImgColors();
        }
        public void CopyHairColor()
        {
            Color c = _uicreator.character.GetPartColor(SlotCategory.Hair, ColorCode.Color1);
            _uicreator.character.SetPartColor(slotCategory, ColorCode.Color1, c);
            refreshImgColors();
        }
        public void CopySkinColor()
        {
            Color c = _uicreator.character.SkinColor;
            _uicreator.character.SetPartColor(slotCategory, ColorCode.Color1, c);
            refreshImgColors();
        }

        private void setState(UIPartColorState state)
        {
            if (_uicreator == null)
                return;

            this.state = state;
            switch (this.state)
            {
                case UIPartColorState.None:
                    refreshImgColors();
                    _uicreator.colorUI.Close();
                    _colorcode = "";
                    setChildActive(true);
                    break;
                case UIPartColorState.Color1:
                    _uicreator.colorUI.Show(_uicreator.character.GetPartColor(slotCategory, ColorCode.Color1));
                    _colorcode = ColorCode.Color1;
                    setChildActive(false);
                    break;
                case UIPartColorState.Color2:
                    _uicreator.colorUI.Show(_uicreator.character.GetPartColor(slotCategory, ColorCode.Color2));
                    _colorcode = ColorCode.Color2;
                    setChildActive(false);
                    break;
                case UIPartColorState.Color3:
                    _uicreator.colorUI.Show(_uicreator.character.GetPartColor(slotCategory, ColorCode.Color3));
                    _colorcode = ColorCode.Color3;
                    setChildActive(false);
                    break;
                default:
                    break;
            }
        }

        private void refreshImgColors()
        {
            if (_uicreator == null)
                return;
            switch (this.mode)
            {
                case UIPartColorMode.OneColor:
                    color1Img.color = _uicreator.character.GetPartColor(slotCategory, ColorCode.Color1);
                    break;
                case UIPartColorMode.ThreeColor:
                    color1Img.color = _uicreator.character.GetPartColor(slotCategory, ColorCode.Color1);
                    color2Img.color = _uicreator.character.GetPartColor(slotCategory, ColorCode.Color2);
                    color3Img.color = _uicreator.character.GetPartColor(slotCategory, ColorCode.Color3);
                    break;
                default:
                    break;
            }
        }

        private void setChildActive(bool isActive)
        {
            for (int i = 0; i < this.transform.childCount; i++)
                this.transform.GetChild(i).gameObject.SetActive(isActive);

            if (!isActive)
                return;
                        
            switch (mode)
            {
                case UIPartColorMode.OneColor:
                    color1Img.gameObject.SetActive(true);
                    color2Img.gameObject.SetActive(false);
                    color3Img.gameObject.SetActive(false);
                    uncolorableText.gameObject.SetActive(false);
                    break;
                case UIPartColorMode.ThreeColor:
                    color1Img.gameObject.SetActive(true);
                    color2Img.gameObject.SetActive(true);
                    color3Img.gameObject.SetActive(true);
                    uncolorableText.gameObject.SetActive(false);
                    break;
                case UIPartColorMode.Uncolorable:
                    color1Img.gameObject.SetActive(false);
                    color2Img.gameObject.SetActive(false);
                    color3Img.gameObject.SetActive(false);
                    uncolorableText.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            hairColor.SetActive(false);
            skinColor.SetActive(false);
            if (slotCategory == SlotCategory.Eyebrow || slotCategory == SlotCategory.FacialHair)
                hairColor.SetActive(true);
            else if (slotCategory == SlotCategory.Mouth)
                skinColor.SetActive(true);
            colorSlider.gameObject.SetActive(slotCategory == SlotCategory.SkinDetails);
        }

        void Update()
        {
            if (state == UIPartColorState.None)
                return;

            if (_uicreator.colorUI.gameObject.activeInHierarchy)
            {
                Color selectedcolor = _uicreator.colorUI.selectedColor;
                if (slotCategory == SlotCategory.SkinDetails)
                    selectedcolor.a = colorSlider.value;
                _uicreator.character.SetPartColor(slotCategory, _colorcode, selectedcolor);
            }
            else
                setState(UIPartColorState.None);
        }
    }
}
