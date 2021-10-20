using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterCreator2D.Utilities.Humanoid
{
    [Serializable]
    public class HumanoidBaker : CharacterBaker
    {
        [SerializeField]
        private SpritePacker m_spritePacker = new SpritePacker();

        private Texture m_capeTexture;
        private Texture m_skirtTexture;

        private List<Renderer> m_characterRenderers = new List<Renderer>();

        private Dictionary<SlotCategory, List<SpritePacker.TextureSprite>> m_packedSlotCategory
            = new Dictionary<SlotCategory, List<SpritePacker.TextureSprite>>();

        public override void BakeSlots(CharacterViewer character)
        {
            RenderSlotsToBakedSlots(character);
            if (PackingBakedSlots(character))
            {
                AssignBakedSlotsToRenderer(character);
                ResourcesManager.ReleaseZeroReference();
            }
            m_capeTexture = null;
            m_skirtTexture = null;
            m_spritePacker.textureSpriteList.Clear();
            m_packedSlotCategory.Clear();
            m_characterRenderers.Clear();
        }

        private void RenderSlotsToBakedSlots(CharacterViewer character)
        {
            foreach (var category in CharacterUtility.slotCategories)
            {
                if (category == SlotCategory.SkinDetails)
                {
                    continue;
                }
                PartSlot partSlot = character.slots.GetSlot(category);
                if (!partSlot.assignedPart)
                {
                    continue;
                }
                RenderTexture texture = HumanoidSlotRenderer.RenderToTexture(character, category);
                if (texture)
                {
                    texture.name = category.ToString();
                }
                switch (category)
                {
                    case SlotCategory.Skirt:
                        m_skirtTexture = texture;
                        break;
                    case SlotCategory.Cape:
                        m_capeTexture = texture;
                        break;
                    default:
                        List<Sprite> sprites = partSlot.assignedPart.sprites;
                        for (int i = 0; i < sprites.Count; i++)
                        {
                            var texSpr = new SpritePacker.TextureSprite()
                            {
                                sourceTexture = texture,
                                sourceSprite = sprites[i]
                            };
                            m_spritePacker.textureSpriteList.Add(texSpr);
                            if (!m_packedSlotCategory.TryGetValue(category, out List<SpritePacker.TextureSprite> result))
                            {
                                result = new List<SpritePacker.TextureSprite>();
                                m_packedSlotCategory[category] = result;
                            }
                            result.Add(texSpr);
                        }
                        break;
                }
            }
        }

        private bool PackingBakedSlots(CharacterViewer character)
        {
            if (!m_spritePacker.Pack())
            {
                Debug.LogWarning("Failed to bake CharacterViewer");
                return false;
            }
            foreach (var category in CharacterUtility.slotCategories)
            {
                switch (category)
                {
                    case SlotCategory.Skirt:
                        m_skirtTexture = GenerateTexture2D(m_skirtTexture as RenderTexture);
                        break;
                    case SlotCategory.Cape:
                        m_capeTexture = GenerateTexture2D(m_capeTexture as RenderTexture);
                        break;
                }
            }
            return true;
        }

        private void AssignBakedSlotsToRenderer(CharacterViewer character)
        {
            character.GetComponentsInChildren(true, m_characterRenderers);
            foreach (var renderer in m_characterRenderers)
            {
                if (renderer is SpriteRenderer sr)
                {
                    sr.color = Color.white;
                }
            }
            foreach (var category in CharacterUtility.slotCategories)
            {
                character.transform.AssignSlotMaterial(category, character.setupData.defaultBakedMaterial, true);
                switch (category)
                {
                    case SlotCategory.SkinDetails:
                        continue;
                    case SlotCategory.Skirt:
                        character.transform.AssignClothTexture(category, m_skirtTexture, false);
                        break;
                    case SlotCategory.Cape:
                        character.transform.AssignClothTexture(category, m_capeTexture, false);
                        break;
                    case SlotCategory.MainHand:
                        {
                            if (!m_packedSlotCategory.TryGetValue(category, out List<SpritePacker.TextureSprite> result))
                            {
                                break;
                            }
                            IEnumerable<Sprite> mainHandSprites = result.Select(psc => psc.resultSprite);
                            character.transform.AssignSprites(SetupData.rWeaponLink, mainHandSprites);
                            character.transform.AssignSpriteColor(SetupData.weaponFXLinks[category], character.slots[category].color1);
                            break;
                        }
                    case SlotCategory.OffHand:
                        {
                            if (!m_packedSlotCategory.TryGetValue(category, out List<SpritePacker.TextureSprite> result))
                            {
                                break;
                            }
                            IEnumerable<Sprite> offHandSprites = result.Select(psc => psc.resultSprite);
                            character.transform.AssignSprites(SetupData.lWeaponLink, offHandSprites);
                            character.transform.AssignSprites(SetupData.bowLink, offHandSprites);
                            character.transform.AssignSprites(SetupData.shieldLink, offHandSprites);
                            character.transform.AssignSpriteColor(SetupData.weaponFXLinks[category], character.slots[category].color1);
                            break;
                        }
                    default:
                        {
                            if (!m_packedSlotCategory.TryGetValue(category, out List<SpritePacker.TextureSprite> result))
                            {
                                break;
                            }
                            if (SetupData.partLinks.TryGetValue(category, out Dictionary<string, string> links))
                            {
                                character.transform.AssignSprites(links, result.Select(psc => psc.resultSprite));
                            }
                            break;
                        }
                }
            }
        }

        private Texture GenerateTexture2D(RenderTexture renderTexture)
        {
            if (!renderTexture)
            {
                return null;
            }
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, m_spritePacker.textureFormat, m_spritePacker.textureMipChain);
            texture.name = renderTexture.name;
            texture.ReadTexture(renderTexture);
            if (m_spritePacker.compressTexture)
            {
                texture.Compress(m_spritePacker.highQualityCompress);
            }
            texture.Apply(m_spritePacker.updateMipMaps, m_spritePacker.makeNoLongerReadable);
            return texture;
        }
    }
}
