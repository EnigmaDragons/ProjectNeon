using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    [Serializable]
    public class SlotList
    {
        /// <summary>
        /// Slot for the armor.
        /// </summary>
        public PartSlot armor;

        /// <summary>
        /// Slot for the boots.
        /// </summary>
        public PartSlot boots;

        /// <summary>
        /// Slot for the ear.
        /// </summary>
        public PartSlot ear;

        /// <summary>
        /// Slot for the eyebrow.
        /// </summary>
        public PartSlot eyebrow;

        /// <summary>
        /// Slot for the eyes.
        /// </summary>
        public PartSlot eyes;

        /// <summary>
        /// Slot for the facial hair.
        /// </summary>
        public PartSlot facialHair;

        /// <summary>
        /// Slot for the gloves.
        /// </summary>
        public PartSlot gloves;

        /// <summary>
        /// Slot for the hair.
        /// </summary>
        public PartSlot hair;

        /// <summary>
        /// Slot for the helmet.
        /// </summary>
        public PartSlot helmet;

        /// <summary>
        /// Slot for the mouth.
        /// </summary>
        public PartSlot mouth;

        /// <summary>
        /// Slot for the nose.
        /// </summary>
        public PartSlot nose;

        /// <summary>
        /// Slot for the pants.
        /// </summary>
        public PartSlot pants;

        /// <summary>
        /// Slot for the skin's details.
        /// </summary>
        public PartSlot skinDetails;

        /// <summary>
        /// Slot for the main hand's weapon.
        /// </summary>
        public PartSlot mainHand;

        /// <summary>
        /// Slot for the off hand's weapon.
        /// </summary>
        public PartSlot offHand;

        /// <summary>
        /// Slot for the cape
        /// </summary>
        public PartSlot cape;

        /// <summary>
        /// Slot for the skirts.
        /// </summary>
        public PartSlot skirts;

        /// <summary>
        /// Slot for the body skin.
        /// </summary>
        public PartSlot bodySkin;

        /// <summary>
        /// Returns PartSlot of a given SlotCategory.
        /// </summary>
        /// <param name="slotCategory">Given SlotCategory.</param>
        /// <returns>PartSlot according to slotCategory.</returns>
        public PartSlot GetSlot(SlotCategory slotCategory)
        {
            switch (slotCategory)
            {
                case SlotCategory.Armor:
                    return this.armor;
                case SlotCategory.Boots:
                    return this.boots;
                case SlotCategory.Ear:
                    return this.ear;
                case SlotCategory.Eyebrow:
                    return this.eyebrow;
                case SlotCategory.Eyes:
                    return this.eyes;
                case SlotCategory.FacialHair:
                    return this.facialHair;
                case SlotCategory.Gloves:
                    return this.gloves;
                case SlotCategory.Hair:
                    return this.hair;
                case SlotCategory.Helmet:
                    return this.helmet;
                case SlotCategory.Mouth:
                    return this.mouth;
                case SlotCategory.Nose:
                    return this.nose;
                case SlotCategory.Pants:
                    return this.pants;
                case SlotCategory.SkinDetails:
                    return this.skinDetails;
                case SlotCategory.MainHand:
                    return this.mainHand;
                case SlotCategory.OffHand:
                    return this.offHand;
                case SlotCategory.Cape:
                    return this.cape;
                case SlotCategory.Skirt:
                    return this.skirts;
                case SlotCategory.BodySkin:
                    return this.bodySkin;
                default:
                    return null;
            }
        }
    }

    [Serializable]
    public class PartSlot
    {
        /// <summary>
        /// Slot's material.
        /// </summary>
        public Material material;

        /// <summary>
        /// Assigned Part.
        /// </summary>
        public Part assignedPart;

        /// <summary>
        /// First color.
        /// </summary>
        public Color color1 = Color.gray;

        /// <summary>
        /// Second color.
        /// </summary>
        public Color color2 = Color.gray;

        /// <summary>
        /// Third color.
        /// </summary>
        public Color color3 = Color.gray;
    }
}