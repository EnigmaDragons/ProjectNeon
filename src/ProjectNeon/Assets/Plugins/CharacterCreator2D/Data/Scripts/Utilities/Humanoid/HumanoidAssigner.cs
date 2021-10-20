using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.Utilities.Humanoid
{
    [Serializable]
    public class HumanoidAssigner : CharacterAssigner
    {
        public override void AssignMaterialToRenderer(CharacterViewer character, SlotCategory slotCategory)
        {
            if (character.IsBaked)
            {
                return;
            }
            character.transform.AssignSlotMaterial(slotCategory, character.slots[slotCategory].material, !Application.isPlaying);
        }

        public override void AssignPartToRenderer(CharacterViewer character, SlotCategory category)
        {
            if (character.IsBaked)
            {
                return;
            }
            switch (category)
            {
                case SlotCategory.MainHand:
                case SlotCategory.OffHand:
                    AssignWeaponToRenderer(character, SlotCategory.MainHand);
                    AssignWeaponToRenderer(character, SlotCategory.OffHand);
                    break;
                case SlotCategory.SkinDetails:
                    AssignSkinDetailsToRenderer(character);
                    break;
                case SlotCategory.Cape:
                case SlotCategory.Skirt:
                    AssignClothToRenderer(character, category);
                    break;
                default:
                    AssignOtherSlotToRenderer(character, category);
                    break;
            }
        }

        public override void AssignColorToRenderer(CharacterViewer character, SlotCategory slotCategory)
        {
            if (character.IsBaked)
            {
                return;
            }
            if (HumanoidUtility.IsSkin(slotCategory))
            {
                return;
            }
            switch (slotCategory)
            {
                case SlotCategory.SkinDetails:
                    AssignSkinDetailsColor(character);
                    break;
                case SlotCategory.MainHand:
                    AssignWeaponFXColor(character, slotCategory);
                    AssignPartColor(character, slotCategory);
                    break;
                case SlotCategory.OffHand:
                    AssignWeaponFXColor(character, slotCategory);
                    AssignPartColor(character, slotCategory);
                    break;
                default:
                    AssignPartColor(character, slotCategory);
                    break;
            }
        }

        public override void AssignSkinColorToRenderer(CharacterViewer character)
        {
            if (character.IsBaked)
            {
                return;
            }
            if (character.skins == null || character.skins.Count == 0)
            {
                character.transform.AssignSkinColor(character.SkinColor);
                return;
            }
            foreach (var skinRenderer in character.skins)
            {
                SpriteRenderer spriteRenderer = skinRenderer as SpriteRenderer;
                if (!spriteRenderer)
                {
                    continue;
                }
                spriteRenderer.color = character.SkinColor;
            }
        }

        public override void AssignTintColorToRenderer(CharacterViewer character)
        {
            if (character.IsBaked)
            {
                foreach (SlotCategory act in CharacterUtility.slotCategories)
                {
                    character.transform.AssignSlotMainColor(act, character.TintColor, false);
                }
            }
            else
            {
                foreach (SlotCategory act in CharacterUtility.slotCategories)
                {
                    character.slots[act].material.AssignColor(character.TintColor);
                }
            }
        }

        private static void AssignWeaponToRenderer(CharacterViewer character, SlotCategory slotCategory)
        {
            PartSlot slot = character.slots.GetSlot(slotCategory);
            Weapon weapon = (Weapon)slot.assignedPart;
            if (!weapon)
            {
                slot.material.AssignColorMaskTexture(null);
                character.transform.AssignWeaponSprites(slotCategory, WeaponCategory.OneHanded, null);
                AssignMuzzlePosition(character, slotCategory, Vector3.zero);
            }
            else
            {
                slot.material.AssignColorMaskTexture(weapon.colorMask);
                character.transform.AssignWeaponSprites(slotCategory, weapon.weaponCategory, weapon.sprites);
                AssignMuzzlePosition(character, slotCategory, weapon.muzzlePosition);
                ResourcesManager.ReleaseZeroReference();
            }
        }

        private static void AssignMuzzlePosition(CharacterViewer character, SlotCategory slotCategory, Vector3 muzzlePosition)
        {
            Transform muzzlefx = null;
            if (slotCategory == SlotCategory.MainHand)
            {
                muzzlefx = character.transform.Find(SetupData.muzzleFXLinks[SlotCategory.MainHand]);
            }
            else if (slotCategory == SlotCategory.OffHand)
            {
                muzzlefx = character.transform.Find(SetupData.muzzleFXLinks[SlotCategory.OffHand]);
            }
            if (muzzlefx)
            {
                muzzlefx.localPosition = muzzlePosition;
            }
        }

        private static void AssignSkinDetailsToRenderer(CharacterViewer character)
        {
            PartSlot slot = character.slots.GetSlot(SlotCategory.SkinDetails);
            Part part = slot.assignedPart;
            if (!part)
            {
                slot.material.AssignSkinDetailsTexture(null);
            }
            else
            {
                slot.material.AssignSkinDetailsTexture(part.texture);
                ResourcesManager.ReleaseZeroReference();
            }
        }

        private static void AssignClothToRenderer(CharacterViewer character, SlotCategory category)
        {
            Transform clothTransform;
            switch (category)
            {
                case SlotCategory.Skirt:
                    clothTransform = character.transform.Find(SetupData.skirtLink);
                    break;
                case SlotCategory.Cape:
                    clothTransform = character.transform.Find(SetupData.capeLink);
                    break;
                default:
                    return;
            }
            if (!clothTransform)
            {
                return;
            }
            PartSlot slot = character.slots.GetSlot(category);
            Part part = slot.assignedPart;
            if (!part)
            {
                slot.material.AssignTexture(null);
                slot.material.AssignColorMaskTexture(null);
                clothTransform.parent.gameObject.SetActive(false);
            }
            else
            {
                slot.material.AssignTexture(part.texture);
                slot.material.AssignColorMaskTexture(part.colorMask);
                ResourcesManager.ReleaseZeroReference();
                clothTransform.parent.gameObject.SetActive(true);
            }
        }

        private static void AssignOtherSlotToRenderer(CharacterViewer character, SlotCategory category)
        {
            PartSlot slot = character.slots.GetSlot(category);
            Part part = slot.assignedPart;
            if (!part)
            {
                slot.material.AssignColorMaskTexture(null);
                character.transform.AssignSlotSprites(category, null);
            }
            else
            {
                slot.material.AssignColorMaskTexture(part.colorMask);
                character.transform.AssignSlotSprites(category, part.sprites);
                ResourcesManager.ReleaseZeroReference();
            }
        }

        private static void AssignWeaponFXColor(CharacterViewer character, SlotCategory slotCategory)
        {
            PartSlot slot = character.slots.GetSlot(slotCategory);
            if (slotCategory == SlotCategory.MainHand || slotCategory == SlotCategory.OffHand)
            {
                character.transform.AssignSpriteColor(SetupData.weaponFXLinks[slotCategory], slot.color1);
            }
        }

        private static void AssignSkinDetailsColor(CharacterViewer character)
        {
            PartSlot slot = character.slots.GetSlot(SlotCategory.SkinDetails);
            slot.material.AssignDetailsColor(slot.color1);
        }

        private static void AssignPartColor(CharacterViewer character, SlotCategory slotCategory)
        {
            PartSlot slot = character.slots.GetSlot(slotCategory);
            if (SetupData.colorableSpriteLinks.TryGetValue(slotCategory, out List<string> links))
            {
                character.transform.AssignSpriteColors(links, slot.color1);
                return;
            }
            slot.material.SetColor(ColorCode.Color1Id, slot.color1);
            slot.material.SetColor(ColorCode.Color2Id, slot.color2);
            slot.material.SetColor(ColorCode.Color3Id, slot.color3);
        }
    }
}
