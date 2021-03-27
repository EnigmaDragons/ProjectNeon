using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UIRandomizeCharacter : MonoBehaviour
    {
        public RandomPackGroup packGroup;

        [Header("Part")]
        public Toggle bodyPartToggle;
        public Toggle facePartToggle;
        public Toggle outfitPartToggle;
        public Toggle weaponPartToggle;
        public Toggle bodySliderToggle;

        [Header("Color")]
        public Toggle bodyColorToggle;
        public Toggle faceColorToggle;
        public Toggle outfitColorToggle;
        public Toggle weaponColorToggle;

        private UICreator _uicreator;

        private void Awake()
        {
            _uicreator = this.GetComponentInParent<UICreator>();
        }

        private static readonly List<SlotCategory> _facecats = new List<SlotCategory>()
        {
            SlotCategory.Ear,
            SlotCategory.Eyebrow,
            SlotCategory.Eyes,
            SlotCategory.FacialHair,
            SlotCategory.Hair,
            SlotCategory.Mouth,
            SlotCategory.Nose
        };

        private static readonly List<SlotCategory> _outfitcats = new List<SlotCategory>()
        {
            SlotCategory.Armor,
            SlotCategory.Boots,
            SlotCategory.Cape,
            SlotCategory.Gloves,
            SlotCategory.Helmet,
            SlotCategory.Pants,
            SlotCategory.Skirt
        };

        private static readonly List<SlotCategory> _weaponcats = new List<SlotCategory>()
        {
            SlotCategory.MainHand,
            SlotCategory.OffHand
        };

        public void Randomize()
        {
            List<SlotCategory> excludedparts = new List<SlotCategory>();
            List<SlotCategory> excludedcolors = new List<SlotCategory>();

            if (_uicreator == null)
                _uicreator = this.GetComponentInParent<UICreator>();

            //randomize parts
            if (!bodyPartToggle.isOn)
                excludedparts.AddRange(new List<SlotCategory>() { SlotCategory.SkinDetails, SlotCategory.BodySkin });
            else
                _uicreator.RandomizeBody();
            if (!facePartToggle.isOn)
                excludedparts.AddRange(_facecats);
            if (!outfitPartToggle.isOn)
                excludedparts.AddRange(_outfitcats);
            if (!weaponPartToggle.isOn)
                excludedparts.AddRange(_weaponcats);
            if (bodySliderToggle.isOn)
                _uicreator.RandomizeBodySliders();

            //randomize colors
            if (!bodyColorToggle.isOn)
                excludedcolors.AddRange(new List<SlotCategory>() { SlotCategory.SkinDetails });
            else
                _uicreator.RandomizeSkinColor();
            if (!faceColorToggle.isOn)
                excludedcolors.AddRange(_facecats);
            if (!outfitColorToggle.isOn)
                excludedcolors.AddRange(_outfitcats);
            if (!weaponColorToggle.isOn)
                excludedcolors.AddRange(_weaponcats);

            _uicreator.RandomizePart(excludedparts, packGroup.GetExcludedPacks());
            _uicreator.RandomizeColor(excludedcolors.ToArray());
        }
    }
}