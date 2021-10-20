//using System;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.Utilities
{
    [Serializable]
    public class SpritePacker
    {
        public class TextureSprite
        {
            public Texture sourceTexture;
            public Sprite sourceSprite;
            public Sprite resultSprite;
        }

        public TextureFormat textureFormat = TextureFormat.RGBAFloat;
        public bool textureMipChain = false;
        public int padding = 8;
        public int atlasSize = 2048;
        public uint spriteExtrude = 1;
        public SpriteMeshType spriteMeshType = SpriteMeshType.FullRect;
        public bool generateFallbackPhysicsShape = false;
        public bool compressTexture = true;
        public bool highQualityCompress = false;
        public bool updateMipMaps = false;
        public bool makeNoLongerReadable = false;
        [Range(0.5f, 0.99999f)]
        public float scaleDownMultiplier = 0.99f;

        public readonly List<TextureSprite> textureSpriteList = new List<TextureSprite>();

        private Vector2[] m_sizes;
        private (Rect, Vector2)[] m_pixelRectAndPivots;
        private readonly List<Rect> m_resultAtlasRects = new List<Rect>(100);

        public bool Pack()
        {
            if (!CalculateSprite())
            {
                return false;
            }
            RenderTexture renderTexture = new RenderTexture(atlasSize, atlasSize, 0);
            Texture2D resultTexture = new Texture2D(atlasSize, atlasSize, textureFormat, textureMipChain)
            {
                name = "AtlasTexture",
                wrapMode = TextureWrapMode.Clamp,
            };
            for (int i = 0; i < textureSpriteList.Count; i++)
            {
                TextureSprite texSpr = textureSpriteList[i];
                if (texSpr == null)
                {
                    continue;
                }
                if (!texSpr.sourceSprite)
                {
                    continue;
                }
                if (!texSpr.sourceTexture)
                {
                    continue;
                }
                Rect targetPixelRect = m_resultAtlasRects[i];
                (Rect, Vector2) pixelRectAndPivot = m_pixelRectAndPivots[i];
                float scalePercent = targetPixelRect.width / pixelRectAndPivot.Item1.width;
                ReadTexture(renderTexture, texSpr.sourceTexture, pixelRectAndPivot.Item1, targetPixelRect);
                Sprite sprite;
                try
                {
                    sprite = Sprite.Create(
                        resultTexture,
                        targetPixelRect,
                        pixelRectAndPivot.Item2,
                        texSpr.sourceSprite.pixelsPerUnit * scalePercent,
                        spriteExtrude,
                        spriteMeshType,
                        texSpr.sourceSprite.border,
                        generateFallbackPhysicsShape);
                    if (sprite)
                    {
                        sprite.name = texSpr.sourceSprite.name;
                        texSpr.resultSprite = sprite;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e.ToString());
                }
            }
            resultTexture.ReadTexture(renderTexture);
            if (compressTexture)
            {
                resultTexture.Compress(highQualityCompress);
            }
            resultTexture.Apply(updateMipMaps, makeNoLongerReadable);
            renderTexture.Release();
            UnityEngine.Object.DestroyImmediate(renderTexture);
            return true;
        }

        private bool CalculateSprite()
        {
            int count = textureSpriteList.Count;
            m_sizes = new Vector2[count];
            m_pixelRectAndPivots = new (Rect, Vector2)[count];
            for (int i = 0; i < count; i++)
            {
                var texSpr = textureSpriteList[i];
                if (texSpr == null)
                {
                    continue;
                }
                var sprite = texSpr.sourceSprite;
                if (!sprite)
                {
                    continue;
                }
                if (sprite.packingMode == SpritePackingMode.Tight)
                {
                    m_pixelRectAndPivots[i].Item1 = sprite.GetTightSpritePixelRect();
                    m_pixelRectAndPivots[i].Item2 = sprite.GetTightSpritePivot(m_pixelRectAndPivots[i].Item1);
                }
                else
                {
                    m_pixelRectAndPivots[i].Item1 = sprite.rect;
                    m_pixelRectAndPivots[i].Item2 = sprite.pivot / sprite.rect.size;
                }
                m_sizes[i] = m_pixelRectAndPivots[i].Item1.size;
            }
            if (!GenerateAtlas(m_sizes, padding, atlasSize, scaleDownMultiplier, m_resultAtlasRects))
            {
                return false;
            }
            return true;
        }

        private static bool GenerateAtlas(Vector2[] sizes, int padding, int atlasSize, float sizeMultiplier, List<Rect> results)
        {
            if (sizes == null)
            {
                return false;
            }
            if (results == null)
            {
                return false;
            }
            if (padding < 0)
            {
                return false;
            }
            if (atlasSize <= 0)
            {
                return false;
            }
            if (sizes.Length == 0)
            {
                return false;
            }
            if (sizeMultiplier <= 0 || sizeMultiplier >= 1)
            {
                return false;
            }
            while (true)
            {
                if (!Texture2D.GenerateAtlas(sizes, padding, atlasSize, results))
                {
                    return false;
                }
                if (sizes[0] != results[0].size)
                {
                    for (int i = 0; i < sizes.Length; i++)
                    {
                        sizes[i] *= sizeMultiplier;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        private static void ReadTexture(RenderTexture renderTexture, Texture sourceTexture, Rect sourcePixelRect, Rect targetPixelRect)
        {
            Rect sourceRect = new Rect(sourcePixelRect.x / sourceTexture.width, sourcePixelRect.y / sourceTexture.height, sourcePixelRect.width / sourceTexture.width, sourcePixelRect.height / sourceTexture.height);
            Rect targetRect = new Rect(targetPixelRect.x / renderTexture.width, targetPixelRect.y / renderTexture.height, targetPixelRect.width / renderTexture.width, targetPixelRect.height / renderTexture.height);
            renderTexture.ReadTexture(sourceTexture, null, sourceRect, targetRect);
        }
    }
}
