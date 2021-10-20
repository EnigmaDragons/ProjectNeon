using System;
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

        private readonly PartSlot _default = new PartSlot();

        public PartSlot this[SlotCategory slotCategory]
        {
            get
            {
                return GetSlot(slotCategory);
            }
        }

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
                    return CheckExistance(ref armor);
                case SlotCategory.Boots:
                    return CheckExistance(ref boots);
                case SlotCategory.Ear:
                    return CheckExistance(ref ear);
                case SlotCategory.Eyebrow:
                    return CheckExistance(ref eyebrow);
                case SlotCategory.Eyes:
                    return CheckExistance(ref eyes);
                case SlotCategory.FacialHair:
                    return CheckExistance(ref facialHair);
                case SlotCategory.Gloves:
                    return CheckExistance(ref gloves);
                case SlotCategory.Hair:
                    return CheckExistance(ref hair);
                case SlotCategory.Helmet:
                    return CheckExistance(ref helmet);
                case SlotCategory.Mouth:
                    return CheckExistance(ref mouth);
                case SlotCategory.Nose:
                    return CheckExistance(ref nose);
                case SlotCategory.Pants:
                    return CheckExistance(ref pants);
                case SlotCategory.SkinDetails:
                    return CheckExistance(ref skinDetails);
                case SlotCategory.MainHand:
                    return CheckExistance(ref mainHand);
                case SlotCategory.OffHand:
                    return CheckExistance(ref offHand);
                case SlotCategory.Cape:
                    return CheckExistance(ref cape);
                case SlotCategory.Skirt:
                    return CheckExistance(ref skirts);
                case SlotCategory.BodySkin:
                    return CheckExistance(ref bodySkin);
                default:
                    Debug.LogWarning(
                        nameof(PartSlot) + " of " +
                        slotCategory + " in " +
                        nameof(SlotList) + " not found! Resturn empty intead.");
                    _default.AssignPart(null);
                    _default.material = null;
                    _default.color1 = Color.gray;
                    _default.color2 = Color.gray;
                    _default.color3 = Color.gray;
                    return _default;
            }
        }

        private PartSlot CheckExistance(ref PartSlot partSlot)
        {
            if (partSlot == null)
            {
                partSlot = new PartSlot();
            }
            return partSlot;
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

        [SerializeField]
        [ReadOnly]
        private string _partName;
        [SerializeField]
        [ReadOnly]
        private string _packageName;

        public string AssignedPartName => _partName;

        public string AssignedPackageName => _packageName;

        public void AssignPart(Part part)
        {
            if (part)
            {
                assignedPart = part;
                _partName = part.name;
                _packageName = part.packageName;
            }
            else
            {
                assignedPart = null;
                _partName = "";
                _packageName = "";
            }
        }
    }
}
