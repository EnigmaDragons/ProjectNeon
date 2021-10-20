using UnityEngine;
using Object = UnityEngine.Object;

namespace CharacterCreator2D.Utilities.Humanoid
{
    public class HumanoidSlotRenderer
    {
        private static readonly HumanoidSlotRenderer m_slotRenderer = new HumanoidSlotRenderer();

        private SlotCategory m_category;
        private CharacterViewer m_character;
        private PartSlot m_partSlot;
        private Part m_part;

        public static Texture2D RenderToTexture2D(CharacterViewer character, SlotCategory slotCategory, TextureFormat textureFormat, bool mipChain)
        {
            RenderTexture renderTexture = m_slotRenderer.RenderSlot(character, slotCategory);
            if (!renderTexture)
            {
                return null;
            }
            Texture2D resultTexture = new Texture2D(renderTexture.width, renderTexture.height, textureFormat, mipChain)
            {
                wrapMode = TextureWrapMode.Clamp,
            };
            resultTexture.ReadTexture(renderTexture);
            resultTexture.Apply();
            renderTexture.Release();
            Object.DestroyImmediate(renderTexture);
            return resultTexture;
        }

        public static RenderTexture RenderToTexture(CharacterViewer character, SlotCategory slotCategory)
        {
            return m_slotRenderer.RenderSlot(character, slotCategory);
        }

        private RenderTexture RenderSlot(CharacterViewer character, SlotCategory category)
        {
            if (!character)
            {
                Debug.LogWarning("Character is null!");
                return null;
            }
            m_partSlot = character.slots.GetSlot(category);
            m_part = m_partSlot.assignedPart;
            if (!m_part)
            {
                m_part = PartList.Static.FindPart(m_partSlot.AssignedPartName, m_partSlot.AssignedPackageName, category);
            }
            if (!m_part)
            {
                return null;
            }

            m_character = character;
            m_category = category;

            // rendering
            Material bakeMaterial = GetMaterialForBaking(m_partSlot.material);
            RenderTexture renderTexture = GetRenderedPartTexture(m_part.texture, bakeMaterial);

            // cleanup
            CharacterUtility.ClearBakeSkinMaterial();
            CharacterUtility.ClearBakeArmorMaterial();
            m_character = null;
            m_partSlot = null;
            m_part = null;
            return renderTexture;
        }

        private Material GetMaterialForBaking(Material sourceMaterial)
        {
            if (!sourceMaterial)
            {
                return null;
            }
            Material bakeMaterial = null;
            if (CharacterUtility.skinShaderNames.Contains(sourceMaterial.shader.name))
            {
                bakeMaterial = CharacterUtility.BakeSkinMaterial;
                if (m_category == SlotCategory.BodySkin)
                {
                    bakeMaterial.SetTexture(CharacterUtility.skinDetailsShaderId, sourceMaterial.GetTexture(CharacterUtility.skinDetailsShaderId));
                    bakeMaterial.SetColor(CharacterUtility.detailsColorShaderId, sourceMaterial.GetColor(CharacterUtility.detailsColorShaderId));
                }
                if (HumanoidUtility.IsSkin(m_category))
                {
                    bakeMaterial.SetColor(CharacterUtility.skinColorShaderId, m_character.SkinColor);
                }
                else
                {
                    bakeMaterial.SetColor(CharacterUtility.skinColorShaderId, m_partSlot.color1);
                }
            }
            else if (CharacterUtility.armorShaderNames.Contains(sourceMaterial.shader.name))
            {
                bakeMaterial = CharacterUtility.BakeArmorMaterial;
                bakeMaterial.SetTexture(CharacterUtility.colorMaskShaderId, m_part.colorMask);
                bakeMaterial.SetColor(ColorCode.Color1Id, m_partSlot.color1);
                bakeMaterial.SetColor(ColorCode.Color2Id, m_partSlot.color2);
                bakeMaterial.SetColor(ColorCode.Color3Id, m_partSlot.color3);
            }
            return bakeMaterial;
        }

        private RenderTexture GetRenderedPartTexture(Texture2D partTexture, Material bakeMaterial)
        {
            RenderTexture targetTexture = new RenderTexture(partTexture.width, partTexture.height, 0)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };
            switch (m_category)
            {
                case SlotCategory.Eyes:
                    {
                        Rect topRect = new Rect(0, 0.5f, 1, 0.5f);
                        Rect bottomRect = new Rect(0, 0, 1, 0.5f);
                        targetTexture.ReadTexture(partTexture, null, topRect, topRect);
                        targetTexture.ReadTexture(partTexture, bakeMaterial, bottomRect, bottomRect);
                    }
                    break;
                case SlotCategory.Mouth:
                    {
                        Rect topRect = new Rect(0, 0.5f, 1, 0.5f);
                        Rect bottomRect = new Rect(0, 0, 1, 0.5f);
                        targetTexture.ReadTexture(partTexture, bakeMaterial, topRect, topRect);
                        targetTexture.ReadTexture(partTexture, null, bottomRect, bottomRect);
                    }
                    break;
                default:
                    targetTexture.ReadTexture(partTexture, bakeMaterial);
                    break;
            }
            return targetTexture;
        }
    }
}
