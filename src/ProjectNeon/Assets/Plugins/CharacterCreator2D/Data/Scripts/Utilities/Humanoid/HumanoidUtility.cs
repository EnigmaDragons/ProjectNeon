using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.Utilities.Humanoid
{
    public static class HumanoidUtility
    {
        public static void AssignWeaponSprites(this Transform transform, SlotCategory category, WeaponCategory weaponCategory, IEnumerable<Sprite> sprites)
        {
            switch (category)
            {
                case SlotCategory.MainHand:
                    AssignMainHandSprites(transform, weaponCategory, sprites);
                    break;
                case SlotCategory.OffHand:
                    AssignOffHandSprites(transform, weaponCategory, sprites);
                    break;
            }
        }

        public static void AssignMainHandSprites(this Transform transform, WeaponCategory weaponCategory, IEnumerable<Sprite> sprites)
        {
            switch (weaponCategory)
            {
                case WeaponCategory.OneHanded:
                case WeaponCategory.Gun:
                case WeaponCategory.TwoHanded:
                case WeaponCategory.Rifle:
                    transform.AssignSprites(SetupData.rWeaponLink, sprites);
                    break;
            }
        }

        public static void AssignOffHandSprites(this Transform transform, WeaponCategory weaponCategory, IEnumerable<Sprite> sprites)
        {
            switch (weaponCategory)
            {
                case WeaponCategory.OneHanded:
                case WeaponCategory.Gun:
                    transform.AssignSprites(SetupData.bowLink, null);
                    transform.AssignSprites(SetupData.shieldLink, null);
                    transform.AssignSprites(SetupData.lWeaponLink, sprites);
                    break;
                case WeaponCategory.Shield:
                    transform.AssignSprites(SetupData.bowLink, null);
                    transform.AssignSprites(SetupData.lWeaponLink, null);
                    transform.AssignSprites(SetupData.shieldLink, sprites);
                    break;
                case WeaponCategory.Bow:
                    transform.AssignSprites(SetupData.shieldLink, null);
                    transform.AssignSprites(SetupData.lWeaponLink, null);
                    transform.AssignSprites(SetupData.bowLink, sprites);
                    break;
            }
        }

        public static void AssignClothTexture(this Transform transform, SlotCategory slotCategory, Texture texture, bool isShared)
        {
            string clothTransformPath;
            switch (slotCategory)
            {
                case SlotCategory.Cape:
                    clothTransformPath = SetupData.capeLink;
                    break;
                case SlotCategory.Skirt:
                    clothTransformPath = SetupData.skirtLink;
                    break;
                default:
                    return;
            }
            if(!transform.GetRenderer(clothTransformPath, out Renderer renderer))
            {
                return;
            }
            Material material;
            if (isShared)
            {
                material = renderer.sharedMaterial;
            }
            else
            {
                material = renderer.material;
            }
            material.AssignTexture(texture);
            renderer.transform.parent.gameObject.SetActive(texture);
        }

        public static void AssignSlotSprites(this Transform transform, SlotCategory category, IEnumerable<Sprite> sprites)
        {
            if (!SetupData.partLinks.TryGetValue(category, out Dictionary<string, string> links))
            {
                return;
            }
            transform.AssignSprites(links, sprites);
        }

        public static void AssignSlotMaterial(this Transform transform, SlotCategory slotCategory, Material material, bool isShared)
        {
            switch (slotCategory)
            {
                case SlotCategory.MainHand:
                    transform.AssignMaterials(SetupData.rWeaponLink.Values, material, isShared);
                    break;
                case SlotCategory.OffHand:
                    transform.AssignMaterials(SetupData.lWeaponLink.Values, material, isShared);
                    transform.AssignMaterials(SetupData.bowLink.Values, material, isShared);
                    transform.AssignMaterials(SetupData.shieldLink.Values, material, isShared);
                    break;
                case SlotCategory.Cape:
                    transform.AssignMaterial(SetupData.capeLink, material, isShared);
                    break;
                case SlotCategory.Skirt:
                    transform.AssignMaterial(SetupData.skirtLink, material, isShared);
                    break;
                default:
                    if (SetupData.partLinks.TryGetValue(slotCategory, out Dictionary<string, string> links))
                    {
                        transform.AssignMaterials(links.Values, material, isShared);
                    }
                    break;
            }
        }

        public static void AssignSlotMainColor(this Transform transform, SlotCategory slotCategory, Color color, bool isShared = true)
        {
            switch (slotCategory)
            {
                case SlotCategory.MainHand:
                    transform.AssignSpriteColors(SetupData.rWeaponLink.Values, color);
                    break;
                case SlotCategory.OffHand:
                    transform.AssignSpriteColors(SetupData.lWeaponLink.Values, color);
                    transform.AssignSpriteColors(SetupData.bowLink.Values, color);
                    transform.AssignSpriteColors(SetupData.shieldLink.Values, color);
                    break;
                case SlotCategory.Cape:
                    transform.AssignColor(SetupData.capeLink, color, isShared);
                    break;
                case SlotCategory.Skirt:
                    transform.AssignColor(SetupData.skirtLink, color, isShared);
                    break;
                default:
                    if (SetupData.partLinks.TryGetValue(slotCategory, out Dictionary<string, string> links))
                    {
                        transform.AssignSpriteColors(links.Values, color);
                    }
                    break;
            }
        }

        public static void AssignSkinColor(this Transform transform, Color color)
        {
            Dictionary<string, string> result;
            if (SetupData.partLinks.TryGetValue(SlotCategory.BodySkin, out result))
            {
                transform.AssignSpriteColors(result.Values, color);
            }
            if (SetupData.partLinks.TryGetValue(SlotCategory.Ear, out result))
            {
                transform.AssignSpriteColors(result.Values, color);
            }
            if (SetupData.partLinks.TryGetValue(SlotCategory.Nose, out result))
            {
                transform.AssignSpriteColors(result.Values, color);
            }
        }

        public static bool IsTwoHandedWeapon(WeaponCategory weaponCategory)
        {
            switch (weaponCategory)
            {
                case WeaponCategory.TwoHanded:
                case WeaponCategory.Bow:
                case WeaponCategory.Rifle:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsMainHandWeapon(WeaponCategory weaponCategory)
        {
            switch (weaponCategory)
            {
                case WeaponCategory.OneHanded:
                case WeaponCategory.Gun:
                case WeaponCategory.TwoHanded:
                case WeaponCategory.Rifle:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOffHandWeapon(WeaponCategory weaponCategory)
        {
            switch (weaponCategory)
            {
                case WeaponCategory.OneHanded:
                case WeaponCategory.Gun:
                case WeaponCategory.Shield:
                case WeaponCategory.Bow:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSkin(SlotCategory slotCategory)
        {
            switch (slotCategory)
            {
                case SlotCategory.BodySkin:
                case SlotCategory.Ear:
                case SlotCategory.Nose:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsFace(SlotCategory slotCategory)
        {
            switch (slotCategory)
            {
                case SlotCategory.Eyebrow:
                case SlotCategory.Eyes:
                case SlotCategory.Nose:
                case SlotCategory.Mouth:
                case SlotCategory.Ear:
                    return true;
                default:
                    return false;
            }
        }
    }
}
