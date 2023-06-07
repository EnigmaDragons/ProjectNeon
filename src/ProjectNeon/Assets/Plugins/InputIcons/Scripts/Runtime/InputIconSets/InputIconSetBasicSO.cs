using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace InputIcons
{
    public abstract class InputIconSetBasicSO : ScriptableObject
    {
        protected enum SearchPattern { One, All };

        public string iconSetName;

        public Color deviceDisplayColor;

        public TMP_FontAsset fontAsset;

        public InputSpriteData unboundData = new InputSpriteData("Unbound Sprite", null, "", "");
        public InputSpriteData fallbackData = new InputSpriteData("Fallback Sprite", null, "FallbackSprite", "");

        public List<CustomInputContextIcon> customContextIcons = new List<CustomInputContextIcon>();

        public abstract string controlSchemeName { get; }



        public bool HasSprite(string bindingTag)
        {
            List<InputSpriteData> spriteData = GetAllInputSpriteData();
            for (int i = 0; i < spriteData.Count; i++)
            {
                if (spriteData[i].textMeshStyleTag.ToUpper() == bindingTag.ToUpper())
                {
                    if (spriteData[i].sprite == null)
                        return false;
                    else
                        return true;
                }
                   
            }

            return false;
        }

        public virtual Sprite GetSprite(string bindingTag)
        {
            List<InputSpriteData> spriteData = GetAllInputSpriteData();
            for (int i = 0; i < spriteData.Count; i++)
            {
                if (spriteData[i].textMeshStyleTag.ToUpper() == bindingTag.ToUpper())
                {
                    return spriteData[i].sprite;
                }

            }

            return unboundData.sprite;
        }

        public abstract void TryGrabSprites();
        public abstract List<InputSpriteData> GetAllInputSpriteData();


        public abstract void ApplyFontCodes(List<KeyValuePair<string, string>> fontCodes);

        protected Sprite GetSpriteFromList(List<Sprite> spriteList, string[] spriteTags, SearchPattern pattern)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                int count = 0;
                for (int j = 0; j < spriteTags.Length; j++)
                {

                    if (spriteList[i].name.ToUpper().Contains(spriteTags[j].ToUpper()))
                    {
                        count++;
                        if (pattern == SearchPattern.One)
                            return spriteList[i];
                    }
                }
                if (pattern == SearchPattern.All && count >= spriteTags.Length)
                    return spriteList[i];
            }

            string s = "";
            for (int i = 0; i< spriteTags.Length; i++)
                s += spriteTags[i].ToString()+" ";

            //InputIconsLogger.Log("Sprite not found, "+ s);
            return null;
        }

        protected List<Sprite> GetSpritesAtPath(string path)
        {
            List<Sprite> sprites = new List<Sprite>();
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets( "t:Sprite", new string[] {path} );


            foreach (string o in guids)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(o).ToString();
                //Debug.Log(spritePath);
                Sprite s = (Sprite)AssetDatabase.LoadAssetAtPath(spritePath, typeof(Sprite));
                sprites.Add(s);
            }
#endif
            return sprites;
        }
    }

    [System.Serializable]
    public class CustomInputContextIcon
    {

        public Sprite customInputContextSprite;
        public string fontCode;
        public string textMeshStyleTag;
    }

    [System.Serializable]
    public class InputSpriteData
    {
        [HideInInspector]
        private string buttonName;
        public string textMeshStyleTag;
        public Sprite sprite;
        public string fontCode;


        public InputSpriteData(string buttonName, Sprite aSprite, string tag, string aFontCode = "")
        {
            this.buttonName = buttonName;
            sprite = aSprite;
            textMeshStyleTag = tag;
            fontCode = aFontCode;
        }

        public void SetFontCode(string code)
        {
            fontCode = code;
        }

        public string GetButtonName()
        {
            if (buttonName == "")
                return textMeshStyleTag;
            return buttonName;
        }

    }
}