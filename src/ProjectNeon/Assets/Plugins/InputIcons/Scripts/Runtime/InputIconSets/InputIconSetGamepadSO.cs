using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace InputIcons
{
    [CreateAssetMenu(fileName = "IconSet", menuName = "Input Icon Set/Gamepad Icon Set", order =502)]
    public class InputIconSetGamepadSO : InputIconSetBasicSO
    {

        public override string controlSchemeName => InputIconsManagerSO.Instance.controlSchemeName_Gamepad;

        [Header("Left Stick")]
        public InputSpriteData lStick;
        public InputSpriteData lStick_Up;
        public InputSpriteData lStick_Down;
        public InputSpriteData lStick_Left;
        public InputSpriteData lStick_Right;
        public InputSpriteData lStick_Click;

        [Header("Right Stick")]
        public InputSpriteData rStick;
        public InputSpriteData rStick_Up;
        public InputSpriteData rStick_Down;
        public InputSpriteData rStick_Left;
        public InputSpriteData rStick_Right;
        public InputSpriteData rStick_Click;

        [Header("D-Pad Buttons")]
        public InputSpriteData dPad;
        public InputSpriteData dPad_Up;
        public InputSpriteData dPad_Down;
        public InputSpriteData dPad_Left;
        public InputSpriteData dPad_Right;

        [Header("Action Buttons")]
        public InputSpriteData north;
        public InputSpriteData south;
        public InputSpriteData west;
        public InputSpriteData east;

        [Header("Triggers")]
        public InputSpriteData l1;
        public InputSpriteData l2;

        public InputSpriteData r1;
        public InputSpriteData r2;

        


        public void InitializeButtons()
        {
            lStick = new InputSpriteData("Left Stick", null, "Left Stick");
            lStick_Up = new InputSpriteData("Left Stick Up", null, "Left Stick/Up");
            lStick_Down = new InputSpriteData("Left Stick Down", null, "Left Stick/Down");
            lStick_Left = new InputSpriteData("Left Stick Left", null, "Left Stick/Left");
            lStick_Right = new InputSpriteData("Left Stick Right", null, "Left Stick/Right");
            lStick_Click = new InputSpriteData("Left Stick Click", null, "Left Stick Press");

            rStick = new InputSpriteData("Right Stick", null, "Right Stick");
            rStick_Up = new InputSpriteData("Right Stick Up", null, "Right Stick/Up");
            rStick_Down = new InputSpriteData("Right Stick Down", null, "Right Stick/Down");
            rStick_Left = new InputSpriteData("Right Stick Left", null, "Right Stick/Left");
            rStick_Right = new InputSpriteData("Right Stick Right", null, "Right Stick/Right");
            rStick_Click = new InputSpriteData("Right Stick Press", null, "Right Stick Press");

            dPad = new InputSpriteData("D-pad", null, "D-Pad");
            dPad_Up = new InputSpriteData("D-pad up", null, "D-Pad/Up");
            dPad_Down = new InputSpriteData("D-pad down", null, "D-Pad/Down");
            dPad_Left = new InputSpriteData("D-pad left", null, "D-Pad/Left");
            dPad_Right = new InputSpriteData("D-pad right", null, "D-Pad/Right");

            north = new InputSpriteData("North", null, "Button North");
            south = new InputSpriteData("South", null, "Button South");
            west = new InputSpriteData("West", null, "Button West");
            east = new InputSpriteData("East", null, "Button East");

            l1 = new InputSpriteData("L1", null, "Left Shoulder");
            l2 = new InputSpriteData("L2", null, "Left Trigger");

            r1 = new InputSpriteData("R1", null, "Right Shoulder");
            r2 = new InputSpriteData("R2", null, "Right Trigger");
        }

        public override List<InputSpriteData> GetAllInputSpriteData()
        {
            List<InputSpriteData> allInputs = new List<InputSpriteData>();

            allInputs.Add(unboundData);
            allInputs.Add(fallbackData);

            allInputs.Add(lStick);
            allInputs.Add(lStick_Up);
            allInputs.Add(lStick_Down);
            allInputs.Add(lStick_Left);
            allInputs.Add(lStick_Right);
            allInputs.Add(lStick_Click);

            allInputs.Add(rStick);
            allInputs.Add(rStick_Up);
            allInputs.Add(rStick_Down);
            allInputs.Add(rStick_Left);
            allInputs.Add(rStick_Right);
            allInputs.Add(rStick_Click);

            allInputs.Add(dPad);
            allInputs.Add(dPad_Up);
            allInputs.Add(dPad_Down);
            allInputs.Add(dPad_Left);
            allInputs.Add(dPad_Right);

            allInputs.Add(north);
            allInputs.Add(south);
            allInputs.Add(west);
            allInputs.Add(east);

            allInputs.Add(l1);
            allInputs.Add(l2);

            allInputs.Add(r1);
            allInputs.Add(r2);

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
            for (int x = 0; x < fontCodes.Count; x++)
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

                for (int i = 0; i < customContextIcons.Count; i++)
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

            string folderName = subpaths[subpaths.Length - 2];
            //Debug.Log("name: "+folderName);

            List<Sprite> sprites = GetSpritesAtPath(folderPath);

            lStick.sprite = GetSpriteFromList(sprites, new string[2] { "Left_Stick_"+ folderName, "Left_Stick" }, SearchPattern.All);
            lStick_Up.sprite = GetSpriteFromList(sprites, new string[1] { "Left_Stick_Up" }, SearchPattern.One);
            lStick_Down.sprite = GetSpriteFromList(sprites, new string[1] { "Left_Stick_Down" }, SearchPattern.One);
            lStick_Left.sprite = GetSpriteFromList(sprites, new string[1] { "Left_Stick_Left" }, SearchPattern.One);
            lStick_Right.sprite = GetSpriteFromList(sprites, new string[1] { "Left_Stick_Right" }, SearchPattern.One);
            lStick_Click.sprite = GetSpriteFromList(sprites, new string[3] { "Left", "Stick", "Click" }, SearchPattern.All);

            rStick.sprite = GetSpriteFromList(sprites, new string[2] { "Right_Stick_"+folderName, "Right_Stick" }, SearchPattern.All);
            rStick_Up.sprite = GetSpriteFromList(sprites, new string[1] { "Right_Stick_Up" }, SearchPattern.One);
            rStick_Down.sprite = GetSpriteFromList(sprites, new string[1] { "Right_Stick_Down" }, SearchPattern.One);
            rStick_Left.sprite = GetSpriteFromList(sprites, new string[1] { "Right_Stick_Left" }, SearchPattern.One);
            rStick_Right.sprite = GetSpriteFromList(sprites, new string[1] { "Right_Stick_Right" }, SearchPattern.One);
            rStick_Click.sprite = GetSpriteFromList(sprites, new string[3] { "Right", "Stick", "Click" }, SearchPattern.All);

            dPad.sprite = GetSpriteFromList(sprites, new string[1] { "dpad" }, SearchPattern.One);
            dPad_Up.sprite = GetSpriteFromList(sprites, new string[1] { "dpad_up" }, SearchPattern.One);
            dPad_Down.sprite = GetSpriteFromList(sprites, new string[1] { "dpad_down" }, SearchPattern.One);
            dPad_Left.sprite = GetSpriteFromList(sprites, new string[1] { "dpad_left" }, SearchPattern.One);
            dPad_Right.sprite = GetSpriteFromList(sprites, new string[1] { "dpad_right" }, SearchPattern.One);


            north.sprite = GetSpriteFromList(sprites, new string[3] { "north", "_Y", "triangle" }, SearchPattern.One);
            south.sprite = GetSpriteFromList(sprites, new string[3] { "south", "_A", "cross" }, SearchPattern.One);
            west.sprite = GetSpriteFromList(sprites, new string[2] { "west", "_X" }, SearchPattern.One); //don't search for 'square' some controllers use a square button for recording
            east.sprite = GetSpriteFromList(sprites, new string[3] { "east", "_B", "circle" }, SearchPattern.One);

            if (iconSetName.Contains("Nintendo") || iconSetName.Contains("Switch"))
            {
                north.sprite = GetSpriteFromList(sprites, new string[3] { "north", "_X", "triangle" }, SearchPattern.One);
                south.sprite = GetSpriteFromList(sprites, new string[3] { "south", "_B", "cross" }, SearchPattern.One);
                west.sprite = GetSpriteFromList(sprites, new string[2] { "west", "_Y" }, SearchPattern.One); //don't search for 'square' some controllers use a square button for recording
                east.sprite = GetSpriteFromList(sprites, new string[3] { "east", "_A", "circle" }, SearchPattern.One);
            }

            if (iconSetName.Contains("PlayStation") || iconSetName.Contains("Play Station") || iconSetName.Contains("PS"))
            {
                west.sprite = GetSpriteFromList(sprites, new string[3] { "west", "_X", "square" }, SearchPattern.One);
            }


            l1.sprite = GetSpriteFromList(sprites, new string[2] { "_l1", "_lb" }, SearchPattern.One);
            l2.sprite = GetSpriteFromList(sprites, new string[2] { "_l2", "_lt" }, SearchPattern.One);

            r1.sprite = GetSpriteFromList(sprites, new string[2] { "_r1", "_rb" }, SearchPattern.One);
            r2.sprite = GetSpriteFromList(sprites, new string[2] { "_r2", "_rt"}, SearchPattern.One);

            InputIconsLogger.Log("Sprites grabbed by sprite names. Please check for correctness.");
            InputIconsLogger.Log("Custom context sprites can not be grabbed automatically.");


            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}