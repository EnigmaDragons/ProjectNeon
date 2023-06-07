#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor.Compilation;
using UnityEngine.TextCore;

namespace InputIcons
{
    public static class InputIconsSpritePacker
    {

        static int MAX_ATLAS_SIZE = 4096;

        public static void PackIconSets()
        {
            InputIconsLogger.Log("Packing icon sets into sprite assets ...");
            List<InputIconSetBasicSO> iconSOs = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();
            //first we have to pack all the textures into the atlas (scaling them by the SCALE value)
            //then we have to create the correct sprite importer settings and add the spritesheet slices to that
            //then we have to set up the settings in the TMP_SpriteAsset

            Object lastEntry = null;

            if (iconSOs[0] != null)
            {
                PackIconSet(iconSOs[0]); //pack first set once first to avoid an annoying bug that would cause the first one to not be properly packed
            }

            for (int i = 0; i < iconSOs.Count; i++)
            {
                if (iconSOs != null)
                {
                    lastEntry = PackIconSet(iconSOs[i]);
                }
            }

            EditorGUIUtility.PingObject(lastEntry);
            CompilationPipeline.RequestScriptCompilation();
        }


        public static Object PackIconSet(InputIconSetBasicSO iconSet)
        {
            Object lastEntry = null;

            if (iconSet != null)
            {
                if(iconSet.iconSetName == "")
                {
                    InputIconsLogger.LogError("Device display name must not be empty. Aborting packing icon set "+iconSet.name, iconSet);
                    return null;
                }

                lastEntry = PackInputIconSetToSpriteAtlasAndAssets(iconSet);
                string fullPath = Path.GetFullPath(AssetDatabase.GetAssetPath(lastEntry));
                fullPath = "Assets" + fullPath.Substring(Application.dataPath.Length);
                string path = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(lastEntry));

                fullPath = fullPath.Replace(path + ".png", "");
                path += ".asset";
                fullPath += path;

                TMP_SpriteAsset spriteAsset = (TMP_SpriteAsset)AssetDatabase.LoadAssetAtPath(fullPath, typeof(TMP_SpriteAsset));
                if (spriteAsset)
                {
                    //adjust placement of glyphs
                    foreach (TMP_SpriteGlyph glyph in spriteAsset.spriteGlyphTable)
                    {
                        glyph.scale *= 1.2f;
                        GlyphMetrics metrics = glyph.metrics;
                        metrics.horizontalBearingX = 0;
                        metrics.horizontalBearingY = glyph.glyphRect.height / 4 * 3;
                        glyph.metrics = metrics;
                    }
                    EditorUtility.SetDirty(spriteAsset);
                }

               

            }
            return lastEntry;
        }

        public static Object PackInputIconSetToSpriteAtlasAndAssets(InputIconSetBasicSO iconSet)
        {

            InputIconsLogger.Log("pack iconset: " + iconSet.iconSetName +"...");

            List<InputSpriteData> spriteDataList = iconSet.GetAllInputSpriteData();
            List<SpriteAssetElement> elements = new List<SpriteAssetElement>();

            for (int i = 0; i < spriteDataList.Count; i++)
            {
                if (spriteDataList[i].sprite == null)
                    continue;

                var element = new SpriteAssetElement();
                elements.Add(element);

                element.path = AssetDatabase.GetAssetPath(spriteDataList[i].sprite.texture);

                element.name = spriteDataList[i].textMeshStyleTag.ToUpper();

                element.sourceTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false, false);
                element.sourceTexture.LoadImage(File.ReadAllBytes(element.path));
                element.sourceTexture.wrapMode = TextureWrapMode.Clamp; //so we don't get pixels from the other edge when scaling
                element.sourceTexture.filterMode = FilterMode.Bilinear;


                element.outputTexture = element.sourceTexture;
            }

            //pack the textures into the atlas
            var textures = elements.ConvertAll<Texture2D>(e => e.outputTexture).ToArray();

            var atlasTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false, false);
            atlasTexture.filterMode = FilterMode.Bilinear;
            var rects = atlasTexture.PackTextures(textures, 0, MAX_ATLAS_SIZE, false);

            float scaleW = (float)atlasTexture.width;
            float scaleH = (float)atlasTexture.height;

            for (int e = 0; e < elements.Count; e++)
            {
                var element = elements[e];
                var rect = rects[e];
                element.rect = rect;

                var pixelRect = new Rect(rect.x * scaleW, rect.y * scaleH, rect.width * scaleW, rect.height * scaleH); //metadata needs pixel rects;

                //https://docs.unity3d.com/ScriptReference/SpriteMetaData.html
                element.meta.name = element.name;
                element.meta.rect = pixelRect;
                element.meta.pivot = new Vector2(0.5f, 0.5f);
                element.meta.border = new Vector4(0, 0, 0, 0);
                element.meta.alignment = 0;

                element.tmpSprite = new TMP_Sprite();
                element.tmpSprite.name = element.name;
                element.tmpSprite.x = pixelRect.x;
                element.tmpSprite.y = pixelRect.y;
                element.tmpSprite.width = pixelRect.width;
                element.tmpSprite.height = pixelRect.height;
                element.tmpSprite.xAdvance = pixelRect.width;
                element.tmpSprite.xOffset = -2f;
                element.tmpSprite.yOffset = pixelRect.height * 0.8f;
                element.tmpSprite.scale = 2.5f;
                element.tmpSprite.id = e;
                element.tmpSprite.hashCode = TMP_TextUtilities.GetSimpleHashCode(element.tmpSprite.name);
            }

            SpriteMetaData[] spriteMetaDatas = elements.ConvertAll<SpriteMetaData>(e => e.meta).ToArray();

            atlasTexture.Apply(false, false);
            string atlasFileName = InputIconsManagerSO.Instance.TEXTMESHPRO_SPRITEASSET_FOLDERPATH +iconSet.iconSetName+  ".png";

            File.WriteAllBytes(atlasFileName, atlasTexture.EncodeToPNG());

            //set up the sprite importer settings
            EditorUtility.SetDirty(atlasTexture);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(atlasFileName);

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.mipmapEnabled = true;
            importer.mipmapFilter = TextureImporterMipFilter.KaiserFilter;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = 4096;
            importer.spritesheet = spriteMetaDatas;

            AssetDatabase.ImportAsset(atlasFileName, ImportAssetOptions.ForceUpdate);

            //cleanup textures
            foreach (var element in elements)
            {
                Texture2D.DestroyImmediate(element.sourceTexture);
                Texture2D.DestroyImmediate(element.outputTexture);
            }

            EditorUtility.SetDirty(atlasTexture);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Object finalTexture = AssetDatabase.LoadAssetAtPath(atlasFileName, typeof(Texture2D));
            Selection.activeObject = finalTexture;
            TMP_SpriteAssetMenu.CreateSpriteAsset();

            /* if TMP_SpriteAssetMenu.CreateSpriteAsset(); can not be accessed, use the following lines instead (requires Reflection)
            System.Type assetMenu = typeof(TMP_SpriteAssetMenu);
            MethodInfo method = assetMenu.GetMethod("CreateSpriteAsset", BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, null);
            */

            return finalTexture;
        }

        //not used anymore, only pack icon sets which are selected on the InputIconSetConfiguratorSO Instance
        /*public static List<InputIconSetBasicSO> GetInputIconSetScriptableObjects() 
        {
            //find input icon scriptable objects
            string[] paths;

            List<InputIconSetBasicSO> iconSOs = new List<InputIconSetBasicSO>();

            paths = AssetDatabase.FindAssets("t:InputIconSetBasicSO");
            foreach (string path in paths)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(path);

                InputIconSetBasicSO iconSO = (InputIconSetBasicSO)AssetDatabase.LoadAssetAtPath(filePath, typeof(InputIconSetBasicSO));
                if (iconSO == null)
                    InputIconsLogger.LogWarning("InputIconSetSO at " + filePath + " could not be loaded.");
                else
                {
                    iconSOs.Add(iconSO);
                }
            }
            return iconSOs;
        }*/

        public class SpriteAssetElement
        {
            public string path;
            public string name;

            public Texture2D sourceTexture;
            public Texture2D outputTexture;

            public Rect rect;
            public SpriteMetaData meta;
            public TMP_Sprite tmpSprite;
        }
    }
}
#endif