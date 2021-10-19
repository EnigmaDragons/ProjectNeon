using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.Utilities
{
    public static class CharacterUtility
    {
        public const string COLOR_MASK_PROP = "_ColorMask";
        public const string SKIN_DETAILS_PROP = "_Details";
        public const string DETAILS_COLOR_PROP = "_DetailsColor";
        public const string SKIN_COLOR_PROP = "_SkinColor";

        private const string BAKED_ARMOR_SHADER_NAME = "Hidden/CC2D/Baked Armor";
        private const string BAKED_SKIN_SHADER_NAME = "Hidden/CC2D/Character Skin Baked";

        public static readonly int colorMaskShaderId = Shader.PropertyToID(COLOR_MASK_PROP);
        public static readonly int skinDetailsShaderId = Shader.PropertyToID(SKIN_DETAILS_PROP);
        public static readonly int detailsColorShaderId = Shader.PropertyToID(DETAILS_COLOR_PROP);
        public static readonly int skinColorShaderId = Shader.PropertyToID(SKIN_COLOR_PROP);

        public static readonly IList<SlotCategory> slotCategories = Array.AsReadOnly(ArrayOfSlotCategory());

        public static readonly ISet<string> armorShaderNames = new HashSet<string>() {
            "CC2D/Unlit/Armor Recolor",
            "CC2D/Lit (Built-in)/Armor Recolor",
            "CC2D/URP/Armor Recolor",
            "CC2D/2DRP (Experimental)/Armor Recolor",
        }.AsReadOnly();

        public static readonly ISet<string> skinShaderNames = new HashSet<string>() {
            "CC2D/Unlit/Character Skin",
            "CC2D/Lit (Built-in)/Character Skin",
            "CC2D/URP/Character Skin",
            "CC2D/2DRP (Experimental)/Character Skin",
        }.AsReadOnly();

        private static readonly List<int> m_texturePropIds = new List<int>();

        private static Material m_bakeSkinMaterial;
        private static Material m_bakeArmorMaterial;

        public static Material BakeSkinMaterial => m_bakeSkinMaterial;
        public static Material BakeArmorMaterial => m_bakeArmorMaterial;

        private static SlotCategory[] ArrayOfSlotCategory()
        {
            SlotCategory[] slotCategoryValues = Enum.GetValues(typeof(SlotCategory)) as SlotCategory[];
            int maxVal = 0;
            for (int i = 0; i < slotCategoryValues.Length; i++)
            {
                maxVal = Mathf.Max(maxVal, (int)slotCategoryValues[i]);
            }
            SlotCategory[] slotCategoriesResult = new SlotCategory[maxVal + 1];
            for (int i = 0; i < slotCategoryValues.Length; i++)
            {
                SlotCategory slotCategory = slotCategoryValues[i];
                slotCategoriesResult[(int)slotCategory] = slotCategory;
            }
            return slotCategoriesResult;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (!m_bakeSkinMaterial)
            {
                m_bakeSkinMaterial = new Material(Shader.Find(BAKED_SKIN_SHADER_NAME));
            }
            if (!m_bakeArmorMaterial)
            {
                m_bakeArmorMaterial = new Material(Shader.Find(BAKED_ARMOR_SHADER_NAME));
            }
        }

        public static bool AssignDetailsColor(this Material material, Color detailsColor)
        {
            return material.AssignColor(detailsColor, detailsColorShaderId);
        }

        public static bool AssignSkinDetailsTexture(this Material material, Texture skinDetails)
        {
            return material.AssignTexture(skinDetails, skinDetailsShaderId);
        }

        public static bool AssignColorMaskTexture(this Material material, Texture colorMask)
        {
            return material.AssignTexture(colorMask, colorMaskShaderId);
        }

        public static void ClearBakeSkinMaterial()
        {
            m_bakeSkinMaterial.GetTexturePropertyNameIDs(m_texturePropIds);
            for (int i = 0; i < m_texturePropIds.Count; i++)
            {
                m_bakeSkinMaterial.SetTexture(m_texturePropIds[i], null);
            }
        }

        public static void ClearBakeArmorMaterial()
        {
            m_bakeArmorMaterial.GetTexturePropertyNameIDs(m_texturePropIds);
            for (int i = 0; i < m_texturePropIds.Count; i++)
            {
                m_bakeArmorMaterial.SetTexture(m_texturePropIds[i], null);
            }
        }
    }
}
