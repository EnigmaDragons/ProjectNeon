using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CharacterEditor2D
{
    public static class SpriteSlicerUtils
    {
        public static List<Sprite> SliceSprite(string sourcePath, string targetPath, params string[] excludedName)
        {
            try
            {
                TextureImporter sourceti = (TextureImporter)AssetImporter.GetAtPath(sourcePath);
                TextureImporter targetti = (TextureImporter)AssetImporter.GetAtPath(targetPath);

                //..reset
                targetti.spriteImportMode = SpriteImportMode.Single;
                targetti.SaveAndReimport();
                //reset..

                targetti.spriteImportMode = sourceti.spriteImportMode;
                List<SpriteMetaData> tempsheet = new List<SpriteMetaData>();

                foreach (SpriteMetaData m in sourceti.spritesheet)
                {
                    if (contains(m.name, excludedName))
                        continue;

                    SpriteMetaData tempsmd = new SpriteMetaData();
                    tempsmd.alignment = m.alignment;
                    tempsmd.border = new Vector4(m.border.x, m.border.y, m.border.z, m.border.w);
                    tempsmd.name = m.name;
                    tempsmd.pivot = new Vector2(m.pivot.x, m.pivot.y);
                    tempsmd.rect = new Rect(m.rect);
                    tempsheet.Add(tempsmd);
                }

                targetti.spritesheet = tempsheet.ToArray();
                targetti.SaveAndReimport();

                Object[] tobj = AssetDatabase.LoadAllAssetsAtPath(targetPath);
                List<Sprite> val = new List<Sprite>();
                foreach (Object o in tobj)
                {
                    if (o is Sprite)
                        val.Add((Sprite)o);
                }

                return val;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return null;
            }
        }

        private static bool contains(string value, string[] listVal)
        {
            foreach (string v in listVal)
            {
                if (value == v)
                    return true;
            }

            return false;
        }

        public static List<string> GetSlicedNames(Texture2D texture)
        {
            if (texture == null)
                return new List<string>();

            List<string> val = new List<string>();
            TextureImporter tempti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
            foreach (SpriteMetaData m in tempti.spritesheet)
                val.Add(m.name);

            return val;
        }
    }
}