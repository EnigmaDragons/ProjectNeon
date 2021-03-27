using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public class PartList : ScriptableObject
    {
        /// <summary>
        /// List of PartPacks.
        /// </summary>
        [Tooltip("List of PartPacks")]
        public List<PartPack> partPacks;

        private static PartList _static = null;

        /// <summary>
        /// Static object of PartList loaded from Resources.
        /// </summary>
        public static PartList Static
        {
            get
            {
                if (_static == null)
                    _static = Resources.Load<PartList>("CC2D_PartList");
                return _static;
            }
            set 
            {
                _static = value;
            }
        }

        /// <summary>
        /// Find Parts from PartList's partPacks.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory</param>
        /// <returns>Returns all Parts supported on a PartSlot of a given SlotCategory.</returns>
        public List<Part> FindParts(SlotCategory slotCategory)
        {
            if (slotCategory == SlotCategory.MainHand)
                return findWeapons(SlotCategory.MainHand);
            else if (slotCategory == SlotCategory.OffHand)
                return findWeapons(SlotCategory.OffHand);
            else
                return FindParts((PartCategory)slotCategory);
        }

        /// <summary>
        /// Find Parts from PartList's partPacks.
        /// </summary>
        /// <param name="partCategory">Desired PartCategory</param>
        /// <returns>Returns all Parts of a given PartCategory.</returns>
        public List<Part> FindParts(PartCategory partCategory)
        {
            foreach (PartPack p in this.partPacks)
            {
                if (p.category == partCategory)
                    return p.parts;
            }
            return new List<Part>();
        }

        private List<Part> findWeapons(SlotCategory slotCategory)
        {
            if (slotCategory < SlotCategory.MainHand)
                return new List<Part>();

            List<Part> val = new List<Part>();
            foreach (PartPack pack in this.partPacks)
            {
                if (pack.category != PartCategory.Weapon)
                    continue;

                foreach (Part part in pack.parts)
                {
                    Weapon weapon = (Weapon)part;
                    if (weapon == null)
                        continue;

                    if (slotCategory == SlotCategory.MainHand)
                    {
                        if (weapon.weaponCategory == WeaponCategory.OneHanded ||
                            weapon.weaponCategory == WeaponCategory.TwoHanded ||
                            weapon.weaponCategory == WeaponCategory.Gun ||
                            weapon.weaponCategory == WeaponCategory.Rifle)
                            val.Add(weapon);
                    }
                    else if (slotCategory == SlotCategory.OffHand)
                    {
                        if (weapon.weaponCategory == WeaponCategory.OneHanded ||
                            weapon.weaponCategory == WeaponCategory.Shield ||
                            weapon.weaponCategory == WeaponCategory.Bow ||
                            weapon.weaponCategory == WeaponCategory.Gun)
                            val.Add(weapon);
                    }
                }

                break;
            }

            return val;
        }

        /// <summary>
        /// Find a Part from this PartList's partPacks.
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="slotCategory"></param>
        /// <returns>First Part found by ignoring the Part's packageName. Returns 'null' if there is no Part found.</returns>
        public Part FindPart(string partName, SlotCategory slotCategory)
        {
            if (slotCategory == SlotCategory.MainHand || slotCategory == SlotCategory.OffHand)
                return FindPart(partName, PartCategory.Weapon);
            else
                return FindPart(partName, (PartCategory)slotCategory);
        }

        /// <summary>
        /// Find a Part from this PartList's partPacks.
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="partCategory"></param>
        /// <returns>First Part found by ignoring the Part's packageName. Return 'null' if there is no Part found.</returns>
        public Part FindPart(string partName, PartCategory partCategory)
        {
			return FindPart (partName, "", partCategory);
        }

        /// <summary>
        /// Find a Part from this PartList's partPacks.
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="partPackage"></param>
        /// <param name="slotCategory"></param>
        /// <returns>May returns 'null' if there is no Part found.</returns>
		public Part FindPart(string partName, string partPackage, SlotCategory slotCategory)
		{
            if (slotCategory == SlotCategory.MainHand || slotCategory == SlotCategory.OffHand)
                return FindPart(partName, partPackage, PartCategory.Weapon);
            else
                return FindPart(partName, partPackage, (PartCategory)slotCategory);
		}

        /// <summary>
        /// Find a Part from this PartList's partPacks.
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="partPackage"></param>
        /// <param name="partCategory"></param>
        /// <returns>May returns 'null' if there is no Part found.</returns>
		public Part FindPart(string partName, string partPackage, PartCategory partCategory)
		{
			foreach (PartPack pack in this.partPacks)
			{
				if (pack.category != partCategory)
					continue;

				foreach (Part part in pack.parts)
				{
					if (!string.IsNullOrEmpty (partPackage) && part.packageName != partPackage)
						continue;
					
					if (part.name == partName)
						return part;
				}
			}
			return null;
		}
    }
}