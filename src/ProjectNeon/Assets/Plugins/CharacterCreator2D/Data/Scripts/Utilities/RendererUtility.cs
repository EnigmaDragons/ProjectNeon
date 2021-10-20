using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

namespace CharacterCreator2D.Utilities
{
    public static class RendererUtility
    {
        public const string MAIN_TEXTURE_PROP = "_MainTex";
        public const string COLOR_PROP = "_Color";
        private const string BLIT_COPY_SHADER_NAME = "Hidden/BlitCopy";

        public static readonly int mainTextureShaderId = Shader.PropertyToID(MAIN_TEXTURE_PROP);
        public static readonly int colorShaderId = Shader.PropertyToID(COLOR_PROP);
        private static Shader m_blitCopyShader;
        private static Material m_defaultMaterial;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (!m_blitCopyShader)
            {
                m_blitCopyShader = Shader.Find(BLIT_COPY_SHADER_NAME);
            }
            if (!m_defaultMaterial)
            {
                var mats = Resources.FindObjectsOfTypeAll<Material>();
                if (mats != null)
                {
                    for (int i = 0; i < mats.Length; i++)
                    {
                        Material mat = mats[i];
                        if (m_blitCopyShader.Equals(mat.shader))
                        {
                            m_defaultMaterial = mat;
                        }
                    }
                }
            }
            if (!m_defaultMaterial)
            {
                m_defaultMaterial = new Material(m_blitCopyShader);
                m_defaultMaterial.name += " For CC2D";
            }
        }

        /// <summary>
        /// Find child transform in <paramref name="transformPath"/> of <paramref name="parentTransform"/>,
        /// Then assign <paramref name="texture"/> to material. If <paramref name="isShared"/> is true, then use sharedMaterial.
        /// </summary>
        /// <param name="parentTransform">Target transform parent</param>
        /// <param name="transformPath">Path of child transform</param>
        /// <param name="texture">Texture to assign</param>
        /// <param name="isShared">If true then use sharedMaterial</param>
        /// <returns>If renderer not found and material is null, then return false</returns>
        public static bool AssignTexture(this Transform parentTransform, string transformPath, Texture texture, bool isShared = true)
        {
            return AssignTexture(parentTransform, transformPath, texture, mainTextureShaderId, isShared);
        }

        /// <summary>
        /// Find child transform in <paramref name="transformPath"/> of <paramref name="parentTransform"/>,
        /// Then assign <paramref name="texture"/> to material. If <paramref name="isShared"/> is true, then use sharedMaterial.
        /// </summary>
        /// <param name="parentTransform">Target transform parent</param>
        /// <param name="transformPath">Path of child transform</param>
        /// <param name="texture">Texture to assign</param>
        /// <param name="propertyId">Property Id of material</param>
        /// <param name="isShared">If true then use sharedMaterial</param>
        /// <returns>If renderer not found and material is null, then return false</returns>
        public static bool AssignTexture(this Transform parentTransform, string transformPath, Texture texture, int propertyId, bool isShared = true)
        {
            if (!GetRenderer(parentTransform, transformPath, out Renderer renderer))
            {
                return false;
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
            return AssignTexture(material, texture, propertyId);
        }

        public static bool AssignTexture(this Material material, Texture mainTexture)
        {
            return AssignTexture(material, mainTexture, mainTextureShaderId);
        }

        public static bool AssignTexture(this Material material, Texture texture, int propertyId)
        {
            if (!material)
            {
                return false;
            }
            material.SetTexture(propertyId, texture);
            return true;
        }

        public static bool AssignColor(this Transform transform, string transformPath, Color color, bool isShared = true)
        {
            return AssignColor(transform, transformPath, color, colorShaderId, isShared);
        }

        public static bool AssignColor(this Transform transform, string transformPath, Color color, int propertyId, bool isShared = true)
        {
            if (!GetRenderer(transform, transformPath, out Renderer renderer))
            {
                return false;
            }
            if (isShared)
            {
                return AssignColor(renderer.sharedMaterial, color, propertyId);
            }
            else
            {
                return AssignColor(renderer.material, color, propertyId);
            }
        }

        public static bool AssignColor(this Material material, Color mainColor)
        {
            return AssignColor(material, mainColor, colorShaderId);
        }

        public static bool AssignColor(this Material material, Color mainColor, int propertyId)
        {
            if (!material)
            {
                return false;
            }
            material.SetColor(propertyId, mainColor);
            return true;
        }

        /// <summary>
        /// Find child transform in <paramref name="transformPath"/> of <paramref name="parentTransform"/>,
        /// then assign <paramref name="sprite"/> to <see cref="SpriteRenderer"/>.
        /// </summary>
        /// <param name="parentTransform">Target transform parent</param>
        /// <param name="transformPath">Path of child transform</param>
        /// <param name="sprite">Sprite to assign</param>
        /// <returns>If <see cref="SpriteRenderer"/> not found, then return false.</returns>
        public static bool AssignSprite(this Transform parentTransform, string transformPath, Sprite sprite)
        {
            if (!GetRenderer(parentTransform, transformPath, out SpriteRenderer renderer))
            {
                return false;
            }
            renderer.sprite = sprite;
            return true;
        }

        /// <summary>
        /// Assign <paramref name="sprites"/> based on <paramref name="links"/> {sprite name, transform path} to child of <paramref name="parentTransform"/>.
        /// All links sprites found will set to null beforehand.
        /// </summary>
        /// <param name="parentTransform">Target transform parent</param>
        /// <param name="links">Links with structure = {nama sprite, transform path}</param>
        /// <param name="sprites">Sprite Collection. Sprite's name should exist in links or it wont assigned</param>
        public static void AssignSprites(this Transform parentTransform, IDictionary<string, string> links, IEnumerable<Sprite> sprites)
        {
            if (links == null)
            {
                Debug.LogWarning("Links is null!");
                return;
            }
            foreach (var link in links)
            {
                AssignSprite(parentTransform, link.Value, null);
            }
            if (sprites == null)
            {
                return;
            }
            foreach (Sprite s in sprites)
            {
                if (!s)
                {
                    continue;
                }
                if (!links.TryGetValue(s.name, out string spritePath))
                {
                    continue;
                }
                AssignSprite(parentTransform, spritePath, s);
            }
        }

        public static bool AssignSpriteColor(this Transform parentTransform, string transformPath, Color color)
        {
            if (!GetRenderer(parentTransform, transformPath, out SpriteRenderer renderer))
            {
                return false;
            }
            renderer.color = color;
            return true;
        }

        public static void AssignSpriteColors(this Transform parentTransform, IEnumerable<string> transformPaths, Color color)
        {
            if (transformPaths == null)
            {
                return;
            }
            foreach (var transformPath in transformPaths)
            {
                AssignSpriteColor(parentTransform, transformPath, color);
            }
        }

        public static bool AssignMaterial(this Transform parentTransform, string transformPath, Material material, bool isShared = true)
        {
            if (!GetRenderer(parentTransform, transformPath, out Renderer renderer))
            {
                return false;
            }
            if (isShared)
            {
                renderer.sharedMaterial = material;
            }
            else
            {
                renderer.material = material;
            }
            return true;
        }

        public static void AssignMaterials(this Transform parentTransform, IEnumerable<string> transformPaths, Material material, bool isShared = true)
        {
            if (transformPaths == null)
            {
                return;
            }
            foreach (string transformPath in transformPaths)
            {
                AssignMaterial(parentTransform, transformPath, material, isShared);
            }
        }

        /// <summary>
        /// Find child transform of <paramref name="parentTransform"/> in <paramref name="transformPath"/> then get <see cref="Renderer"/> from it.
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="Renderer"/></typeparam>
        /// <param name="parentTransform">Target transform parent</param>
        /// <param name="transformPath">Path of child transform</param>
        /// <param name="result">Renderer with type of <typeparamref name="T"/></param>
        /// <returns>If <paramref name="parentTransform"/>==null or child transform not found or no renderer in child transform or type mismatch,
        /// then return false.</returns>
        public static bool GetRenderer<T>(this Transform parentTransform, string transformPath, out T result) where T : Renderer
        {
            result = null;
            if (!GetTransformAtPath(parentTransform, transformPath, out Transform foundTransform))
            {
                return false;
            }
            Renderer renderer = foundTransform.GetComponent<Renderer>();
            if (!renderer)
            {
                Debug.LogWarning("No renderer attached in " + transformPath, renderer);
                return false;
            }
            result = renderer as T;
            if (!result)
            {
                Debug.LogWarning("Renderer type mismatch!", renderer);
                return false;
            }
            return true;
        }

        public static bool GetTransformAtPath(this Transform parentTransform, string transformPath, out Transform result)
        {
            result = null;
            if (!parentTransform)
            {
                Debug.LogWarning("Transform parent is null");
                return false;
            }
            result = parentTransform.Find(transformPath);
            if (!result)
            {
                Debug.LogWarning("Transform " + transformPath + " not found", parentTransform);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Clear <paramref name="renderTexture"/>. Internaly call <see cref="GL"/>.<see cref="GL.Clear(bool, bool, Color, float)"/> to <paramref name="renderTexture"/>.
        /// </summary>
        /// <param name="renderTexture">Texture that will be cleared</param>
        /// <param name="clearDepth"></param>
        /// <param name="clearColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="depth"></param>
        public static void ClearTexture(this RenderTexture renderTexture, bool clearDepth, bool clearColor, Color backgroundColor, int depth = 0)
        {
            if (!renderTexture)
            {
                Debug.LogWarning("RenderTexture is null");
                return;
            }
            RenderTexture lastRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(clearDepth, clearColor, backgroundColor, depth);
            RenderTexture.active = lastRT;
        }

        /// <summary>
        /// Read pixels from <paramref name="renderTexture"/>.
        /// </summary>
        /// <param name="texture">Texture that read the texture</param>
        /// <param name="renderTexture">Source texture to read from</param>
        public static void ReadTexture(this Texture2D texture, RenderTexture renderTexture)
        {
            if (!texture)
            {
                Debug.LogWarning("Texture is null");
                return;
            }
            if (!renderTexture)
            {
                return;
            }
            RenderTexture lastRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, true);
            RenderTexture.active = lastRT;
        }

        /// <summary>
        /// Read <see cref="Texture"/> from another <paramref name="sourceTexture"/> with <paramref name="material"/>'s pass.
        /// </summary>
        /// <param name="renderTexture">Texture that read the texture</param>
        /// <param name="sourceTexture">Source texture to read from</param>
        /// <param name="material">Material to pass while reading texture</param>
        public static void ReadTexture(this RenderTexture renderTexture, Texture sourceTexture, Material material)
        {
            if (!renderTexture)
            {
                Debug.LogWarning("RenderTexture is null");
                return;
            }
            if (renderTexture == sourceTexture)
            {
                return;
            }
            if (!material && !sourceTexture)
            {
                return;
            }
            ReadTextureInternal(renderTexture, sourceTexture, material, new Rect(0, 0, 1, 1), new Rect(0, 0, 1, 1));
        }

        /// <summary>
        /// Read <see cref="Texture"/> from another <paramref name="sourceTexture"/> with <paramref name="material"/>'s pass.
        /// </summary>
        /// <param name="renderTexture">Texture that read the texture</param>
        /// <param name="sourceTexture">Source texture to read from</param>
        /// <param name="material">Material to pass while reading texture</param>
        /// <param name="sourceRect">Offset and Scale of source texture</param>
        /// <param name="targetRect">Offset and Scale of tarrget texture</param>
        public static void ReadTexture(this RenderTexture renderTexture, Texture sourceTexture, Material material, Rect sourceRect, Rect targetRect)
        {
            if (!renderTexture)
            {
                Debug.LogWarning("RenderTexture is null");
                return;
            }
            if (renderTexture == sourceTexture)
            {
                return;
            }
            if (!material && !sourceTexture)
            {
                return;
            }
            ReadTextureInternal(renderTexture, sourceTexture, material, sourceRect, targetRect);
        }

        private static void ReadTextureInternal(RenderTexture renderTexture, Texture sourceTexture, Material material, Rect sourceRect, Rect targetRect)
        {
            RenderTexture lastRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            {
                if (!material)
                {
                    material = m_defaultMaterial;
                }
                Texture lastTexture = material.mainTexture;
                material.mainTexture = sourceTexture;
                {
                    material.SetPass(0);
                    GL.PushMatrix();
                    GL.LoadOrtho();
                    GL.Begin(GL.QUADS);
                    GL.TexCoord2(sourceRect.xMin, sourceRect.yMin);
                    GL.Vertex(new Vector2(targetRect.xMin, targetRect.yMin));
                    GL.TexCoord2(sourceRect.xMin, sourceRect.yMax);
                    GL.Vertex(new Vector2(targetRect.xMin, targetRect.yMax));
                    GL.TexCoord2(sourceRect.xMax, sourceRect.yMax);
                    GL.Vertex(new Vector2(targetRect.xMax, targetRect.yMax));
                    GL.TexCoord2(sourceRect.xMax, sourceRect.yMin);
                    GL.Vertex(new Vector2(targetRect.xMax, targetRect.yMin));
                    GL.End();
                    GL.PopMatrix();
                }
                material.mainTexture = lastTexture;
            }
            RenderTexture.active = lastRT;
        }

        public static Rect GetTightSpritePixelRect(this Sprite sprite)
        {
            if (!sprite)
            {
                Debug.LogWarning("Sprite is null");
                return default;
            }
            Vector4 padding = DataUtility.GetPadding(sprite);
            Rect spritePixelRect = sprite.rect;
            return new Rect()
            {
                x = spritePixelRect.x + padding.x,
                y = spritePixelRect.y + padding.y,
                xMax = spritePixelRect.xMax - padding.z,
                yMax = spritePixelRect.yMax - padding.w,
            };
        }

        public static Vector2 GetTightSpritePivot(this Sprite sprite)
        {
            return GetTightSpritePivot(sprite, GetTightSpritePixelRect(sprite));
        }

        public static Vector2 GetTightSpritePivot(this Sprite sprite, Rect tightRect)
        {
            if (!sprite)
            {
                Debug.LogWarning("Sprite is null");
                return new Vector2(0.5f, 0.5f);
            }
            return (sprite.rect.position + sprite.pivot - tightRect.position) / tightRect.size;
        }
    }
}
