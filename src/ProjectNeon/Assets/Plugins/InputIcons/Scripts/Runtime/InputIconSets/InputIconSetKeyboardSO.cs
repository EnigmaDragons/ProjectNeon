using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System;
using System.Text;

namespace InputIcons
{
    [CreateAssetMenu(fileName = "IconSet", menuName = "Input Icon Set/Keyboard Icon Set", order = 501)]
    public class InputIconSetKeyboardSO : InputIconSetBasicSO
    {

        public override string controlSchemeName => InputIconsManagerSO.Instance.controlSchemeName_Keyboard;

        public InputSpriteData mouse;
        public InputSpriteData mouse_left;
        public InputSpriteData mouse_right;
        public InputSpriteData mouse_middle;


        public List<InputSpriteData> inputKeys = new List<InputSpriteData>();


        private void Awake()
        {
            InitializeMouseButtons();
        }

        public void InitializeMouseButtons()
        {
            mouse = new InputSpriteData("Mouse Simple", mouse.sprite, "Delta", mouse.fontCode);
            mouse_left = new InputSpriteData("Mouse Left", mouse_left.sprite, "LeftButton", mouse_left.fontCode);
            mouse_right = new InputSpriteData("Mouse Right", mouse_right.sprite, "RightButton", mouse_right.fontCode);
            mouse_middle = new InputSpriteData("Mouse Middle", mouse_middle.sprite, "MiddleButton", mouse_middle.fontCode);

        }

        public override List<InputSpriteData> GetAllInputSpriteData()
        {
            List <InputSpriteData> allInputs = new List<InputSpriteData>(inputKeys);
            allInputs.Add(unboundData);
            allInputs.Add(fallbackData);

            allInputs.Add(mouse);
            allInputs.Add(mouse_left);
            allInputs.Add(mouse_right);
            allInputs.Add(mouse_middle);

            for (int i = 0; i < customContextIcons.Count; i++)
            {
                allInputs.Add(new InputSpriteData(customContextIcons[i].textMeshStyleTag,
                    customContextIcons[i].customInputContextSprite,
                    customContextIcons[i].textMeshStyleTag, customContextIcons[i].fontCode));
            }

            return allInputs;
        }


        public override void ApplyFontCodes(List<KeyValuePair<string, string>> fontCodes)
        {
            List<InputSpriteData> data = GetAllInputSpriteData();
            for(int x=0; x < fontCodes.Count; x++)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (fontCodes[x].Key == data[i].textMeshStyleTag)
                    {
                        //Debug.Log("apply code: " + fontCodes[x].Value + "   earlierCode: " + data[i].fontCode);
                        //Debug.Log("code now: " + data[i].textMeshStyleTag);
                        data[i].SetFontCode(fontCodes[x].Value);
                        //Debug.Log("code now: " + data[i].fontCode);
                    }
                }

                for(int i=0; i<customContextIcons.Count; i++)
                {
                    if (fontCodes[x].Key == customContextIcons[i].textMeshStyleTag)
                    {
                        customContextIcons[i].fontCode = fontCodes[x].Value;

                    }
                }
            }
        }


        public override void TryGrabSprites()
        {
#if UNITY_EDITOR
            string folderPath = AssetDatabase.GetAssetPath(this);

            string[] subpaths = folderPath.Split('/');
            folderPath = "";
            for (int i = 0; i < subpaths.Length - 1; i++)
            {
                folderPath += subpaths[i] + "/";

            }

            List<Sprite> sprites = GetSpritesAtPath(folderPath);

            //Mouse buttons
            mouse.sprite = GetSpriteFromList(sprites, new string[1] { "Mouse_Simple" }, SearchPattern.One);
            mouse_left.sprite = GetSpriteFromList(sprites, new string[1] { "Mouse_Left" }, SearchPattern.One);
            mouse_right.sprite = GetSpriteFromList(sprites, new string[1] { "Mouse_Right" }, SearchPattern.One);
            mouse_middle.sprite = GetSpriteFromList(sprites, new string[1] { "Mouse_Middle" }, SearchPattern.One);

            //Keyboard buttons
            inputKeys = new List<InputSpriteData>();

            string[] keyNames = System.Enum.GetNames (typeof(Key));
            int c = 0;
            for (int i = 0; i < keyNames.Length; i++)
            {
                Sprite s = GetKeySprite(sprites, keyNames[i]);
                string tmpTag =  keyNames[i];

                if (keyNames[i].StartsWith("OEM"))
                {
                    s = null;
                }
                 

                inputKeys.Add(new InputSpriteData(keyNames[i], s, tmpTag, ""));
                if (s != null)
                    c++;
            }

            InputIconsLogger.Log("found keys: " + c);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private Sprite GetKeySprite(List<Sprite> sprites, string keyName)
        {
            string searchName = keyName+"_Key";

            //search for numbers
            if (keyName.StartsWith("Digit"))
            {
                string number = keyName.Substring(5);
                //InputIconsLogger.Log("searched number = " + number);
                searchName = number + "_key";
            }

            //search for arrows
            if (keyName.Contains("Arrow"))
            {
                string direction = keyName;
                direction = direction.Replace("Arrow", "");
                direction = direction.ToUpper();
                //InputIconsLogger.Log("searched arrow = " + direction);
                searchName = "ARROW_" + direction + "_KEY";
            }

            //shift keys
            //if (keyName.Contains("Shift"))
            //{
            //    searchName = "shift_key";
            //    if (keyName.StartsWith("Left"))
            //    {
            //        searchName = "shift_alt_key";
            //    }
            //}

            //ctrl keys
            if (keyName.Contains("Ctrl"))
            {
                searchName = "ctrl_key";
            }

            //alt keys
            if (keyName.Contains("Alt"))
            {
                searchName = "alt_key";
            }

            //delete key
            if (keyName.Equals("Delete"))
            {
                searchName = "del_key";
            }

            //keys that vary depending on keyboard manufactory
            if(keyName.StartsWith("OEM") || keyName.Contains("Meta") || keyName.Contains("ContextMenu")
                || keyName.Contains("Windows") || keyName.Contains("Apple") || keyName.Contains("Command"))
            {
                searchName = "undefined_key";
            }


            searchName = searchName.ToUpper();
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].name.ToUpper().StartsWith(searchName))
                {
                    return sprites[i];
                }
            }


            return null;
        }
    }
}