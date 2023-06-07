using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using System;
using System.IO;

namespace InputIcons
{
    public class InputIconsSetupWindow : EditorWindow
    {
        Vector2 scrollPos;

        bool showPart1 = false;
        bool showPart2 = false;
        bool showPart3 = false;

        bool showCopyFontsPart = false;

        bool showAdvanced = false;
        GUIStyle textStyleHeader;
        GUIStyle textStyle;
        GUIStyle textStyleYellow;
        GUIStyle textStyleBold;
        GUIStyle buttonStyle;

        private Editor editor;


        private List<InputIconSetBasicSO> iconSetSOs;
        //public List<InputActionAsset> usedInputActionAssets;

        private InputIconsManagerSO managerSO;

        public static SerializedObject serializedManager;
        public static SerializedProperty serializedInputActionAssets;

        public GameObject activationPrefab;

        [MenuItem("Tools/Input Icons/Input Icons Setup", priority = 0)]
        public static void ShowWindow()
        {
            const int width = 800;
            const int height = 730;

            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;

            GetWindow<InputIconsSetupWindow>("Input Icons Setup").iconSetSOs = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();
            EditorWindow window = GetWindow<InputIconsSetupWindow>("Input Icons Setup");
            window.position = new Rect(x, y, width, height);
        }

        protected void OnEnable()
        {
            // load values
            var data = EditorPrefs.GetString("InputIconsSetupWindow", JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);

            position.Set(position.x, position.y, 800, 700);

            managerSO = InputIconsManagerSO.Instance;

            serializedManager = new SerializedObject(InputIconsManagerSO.Instance);
            serializedInputActionAssets = serializedManager.FindProperty("usedActionAssets");

            activationPrefab = Resources.Load("InputIcons/II_InputIconsActivator") as GameObject;

            //do this so the list does not appear null ... weird bug that otherwise can happen when package just got imported
            serializedInputActionAssets.InsertArrayElementAtIndex(0);
            var elementProperty = serializedInputActionAssets.GetArrayElementAtIndex(0);
            if(elementProperty!=null)
                elementProperty.objectReferenceValue = null;
            serializedInputActionAssets.DeleteArrayElementAtIndex(0);

        }

        protected void OnDisable()
        {
            // save values
            var data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString("InputIconsSetupWindow", data);
        }

        private bool AllInputActionAssetsNull()
        {
            if (serializedInputActionAssets.arraySize == 0)
                return true;

            for(int i=0; i< serializedInputActionAssets.arraySize; i++)
            {
                if (serializedInputActionAssets.GetArrayElementAtIndex(i).objectReferenceValue as System.Object as InputActionAsset != null)
                    return false;
            }

            return true;

        }
        private void OnGUI()
        {

            textStyleHeader = new GUIStyle(EditorStyles.boldLabel);
            textStyleHeader.wordWrap = true;
            textStyleHeader.fontSize = 14;

            textStyle = new GUIStyle(EditorStyles.label);
            textStyle.wordWrap = true;

            textStyleYellow = new GUIStyle(EditorStyles.label);
            textStyleYellow.wordWrap = true;
            textStyleYellow.normal.textColor = Color.yellow;

            textStyleBold = new GUIStyle(EditorStyles.boldLabel);
            textStyleBold.wordWrap = true;

            buttonStyle = EditorStyles.miniButtonMid;

            scrollPos =
               EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));

            managerSO = (InputIconsManagerSO)EditorGUILayout.ObjectField("", managerSO, typeof(InputIconsManagerSO), true);
            if (managerSO == null)
            {
                EditorGUILayout.HelpBox("Select the icon manager.", MessageType.Warning);
                EditorGUILayout.EndScrollView();
                return;
            }

            GUILayout.Space(20);


#if !ENABLE_INPUT_SYSTEM
            // New input system backends are enabled.
                        EditorGUILayout.HelpBox("Enable the new Input System in Project Settings for full functionality.\n" +
                "Project Settings -> Player -> Other Settings. Set Active Input Handling to 'Input System Package (new)' or 'Both'.", MessageType.Warning);
#endif

            managerSO.TEXTMESHPRO_SPRITEASSET_FOLDERPATH = EditorGUILayout.TextField("Default Sprite Asset folder: ", managerSO.TEXTMESHPRO_SPRITEASSET_FOLDERPATH);

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(serializedInputActionAssets);
            GUILayout.Label("Note, when you make changes to your Input Action Assets like adding actions, you will need" +
                "to create the necessary styles for the new actions. You can easily do this in the Manual Setup or in the Quick Setup section.", textStyle);

            if (AllInputActionAssetsNull())
            {
                EditorGUILayout.HelpBox("Select an Input Asset before you continue.", MessageType.Warning);
                EditorGUILayout.EndScrollView();
                serializedManager.ApplyModifiedProperties();
                return;
            }

            GUILayout.Space(10);
            DrawUILine(Color.grey);
            DrawUILine(Color.grey);

            DrawControlSchemePart();


            GUILayout.Space(10);
            DrawUILine(Color.grey);

            DrawFontPart();
            

            GUILayout.Space(10);
            DrawUILine(Color.grey);
            DrawUILine(Color.grey);

            EditorGUILayout.BeginHorizontal();
            
            //Quick Setup
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2));

            DrawQuickSetup();

            EditorGUILayout.EndVertical();

            
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            EditorGUILayout.EndVertical();

            DrawUILineVertical(Color.grey);

            EditorGUILayout.BeginVertical(GUILayout.Width(10));
            EditorGUILayout.EndVertical();


            //manual setup
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Manual Setup", textStyleHeader);
            GUILayout.Label("To setup this tool manually, complete the following steps.\n" +
                "If this is the first time you setup this tool, complete all steps.\n" +
                "If the tool is already setup and you want to make updates to the sprites or the " +
                "TMPro default style sheet, do the parts you need.", textStyle);

            GUILayout.Space(10);

            DrawCustomPartPackSpriteAssets();

            GUILayout.Space(5);
            DrawCustomPartCopyFontAssetsAssets();
            GUILayout.Space(5);

            DrawCustomPartStyleSheetUpdate();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("Finally add the activation prefab to your first scene.", textStyleHeader);
            GUILayout.Label("This will ensure that the InputIconsManager will be active in our builds " +
                "and will update the displayed icons when needed.", textStyle);
            EditorGUI.BeginDisabledGroup(true);
            activationPrefab = (GameObject)EditorGUILayout.ObjectField("", activationPrefab, typeof(GameObject), true);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);
            DrawUILine(Color.grey);
            DrawUILine(Color.grey);
            GUILayout.Space(10);

            DrawBottomPart();

            GUILayout.Space(5);
            DrawUILine(Color.grey);
            DrawUILine(Color.grey);
            GUILayout.Space(5);

            DrawAdvanced();
            
            EditorGUILayout.EndScrollView();

            serializedManager.ApplyModifiedProperties();
        }

        private void DrawFontPart()
        {
            GUILayout.Label("(Optional) Enable font styles as well.", textStyleBold);
            GUILayout.Label("Enabling this allows you to display Input Icons as a SDF font in TMPro texts as well.\n" +
                "If enabled, additional styles for fonts will be created in the default style sheet during the setup process.", textStyle);

            managerSO.TEXTMESHPRO_FONTASSET_FOLDERPATH = EditorGUILayout.TextField("Default Sprite Asset folder: ", managerSO.TEXTMESHPRO_FONTASSET_FOLDERPATH);
            managerSO.isUsingFonts = EditorGUILayout.Toggle("(Optional) Use fonts", managerSO.isUsingFonts);

            if (managerSO.isUsingFonts && !CheckAllSelectedIconSetsHaveFontsAssigned())
            {
                GUILayout.Label("WARNING: not all Icon Sets on InputIconsConfigurator have a font asset assigned.\n" +
                    "Make sure each has one before you do the setup. Assign font assets and do the Quick Setup (or in the manual setup the Update TMPro Style Sheet - part) again to ensure functionality.", textStyleYellow);

                InputIconSetConfiguratorSO.Instance = (InputIconSetConfiguratorSO)EditorGUILayout.ObjectField("", InputIconSetConfiguratorSO.Instance, typeof(InputIconSetConfiguratorSO), true);
            }
        }

        private bool CopyObjectToDefaultFontsFolder(UnityEngine.Object obj)
        {
            if(obj != null)
            {
                string sourcePath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(sourcePath))
                {
                    string destinationFolderPath = managerSO.TEXTMESHPRO_FONTASSET_FOLDERPATH + obj.name+".asset";

                    if (File.Exists(destinationFolderPath))
                    {
                        FileUtil.DeleteFileOrDirectory(destinationFolderPath);
                        AssetDatabase.Refresh();
                    }

                    FileUtil.CopyFileOrDirectory(sourcePath, destinationFolderPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return true;
                }
            }
            return false;
        }

        private void CopyUsedFontAssetsToTMProDefaultFolder()
        {
            InputIconsLogger.Log("Copying font assets ...");
            List<InputIconSetBasicSO> iconSets = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();
            int c = 0;
            for (int i = 0; i < iconSets.Count; i++)
            {
                if (iconSets[i] == null)
                    continue;
                if (iconSets[i].fontAsset == null)
                    continue;

                if (CopyObjectToDefaultFontsFolder(iconSets[i].fontAsset))
                    c++;
            }
            InputIconsLogger.Log(c+ " font assets copied and ready to be referenced in TMPro text fields");
        }

        private bool CheckAllSelectedIconSetsHaveFontsAssigned()
        {
            List<InputIconSetBasicSO> iconSets = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();
            for(int i=0; i<iconSets.Count; i++)
            {
                if (iconSets[i] != null)
                {
                    if (iconSets[i].fontAsset == null)
                        return false;
                }
            }
            return true;
        }

        private void DrawQuickSetup()
        {
            GUILayout.Label("Quick Setup", textStyleHeader);

            if(AllInputActionAssetsNull())
            {
                EditorGUILayout.HelpBox("Select an Input Asset before you continue.", MessageType.Warning);
            }
            else
            {
                GUILayout.Space(3);

                GUILayout.Label("To setup this tool with the standard functionality, use the buttons below. " +
                    "Wait for Unity to recompile after the first button press.", textStyle);

                if (GUILayout.Button("Step 1: Create Sprite Assets (and font assets if enabled above)\n" +
                    "and prepare Style Sheet with empty values\n" +
                    "(then wait for compilation)"))
                {
                    if(managerSO.isUsingFonts)
                    {
                        CopyUsedFontAssetsToTMProDefaultFolder();
                    }

                    InputIconsSpritePacker.PackIconSets();
                    InputIconsLogger.Log("Packing button icons completed");

                    InputIconsLogger.Log("Preparing default TMP style sheet for additional entries ...");

                    managerSO.CreateInputStyleData();
                    int c = 0;
                    c += InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleKeyboardDataList);
                    c += InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleGamepadDataList);
             
                    Debug.Log("TMP style sheet prepared with "+c+" empty values.");
                    if(c==0)
                    {
                        InputIconsLogger.LogWarning(c + " empty entries added which is generally not expected. Try the same step again.");
                    }

                    CompilationPipeline.RequestScriptCompilation();
                }

                if (!EditorApplication.isCompiling)
                {
                    if (GUILayout.Button("Step 2: Add Input Action names to Style Sheet"))
                    {
                        InputIconsLogger.Log("Adding entries to default TMP style sheet ...");

                        managerSO.CreateInputStyleData();
                        int c = 0;
                        c += InputIconsManagerSO.AddInputStyles(managerSO.inputStyleKeyboardDataList);
                        c += InputIconsManagerSO.AddInputStyles(managerSO.inputStyleGamepadDataList);

                        InputIconsLogger.Log("TMP style sheet updated with ("+ c+ ") styles (multiple entries combined to only one)");
                        
                        InputIconsManagerSO.UpdateTMProStyleSheetWithUsedPlayerInputs();
                        TMP_InputStyleHack.RemoveEmptyEntriesInStyleSheet();
                        //TMP_InputStyleHack.RefreshAllTMProUGUIObjects();
                    }
                }
                else
                {

                    GUILayout.Label("... waiting for compilation ...", textStyleYellow);
                }
            }
        }


        private void DrawControlSchemePart()
        {
            GUILayout.Label("First things first: Control scheme names", textStyleHeader);
            GUILayout.Label("To avoid complications later on, let's set this up first.", textStyle);

            GUILayout.Label("Make sure the names of the " +
               "control schemes for keyboard and gamepad are equal to the names of the control schemes you have set in the " +
               "Input Action Asset(s).", textStyle);
            if (managerSO)
            {
                managerSO.controlSchemeName_Keyboard = EditorGUILayout.TextField("Keyboard Control Scheme Name", managerSO.controlSchemeName_Keyboard);
                managerSO.controlSchemeName_Gamepad = EditorGUILayout.TextField("Gamepad Control Scheme Name", managerSO.controlSchemeName_Gamepad);
            }
           
        }

        private void DrawCustomPartPackSpriteAssets()
        {
            //GUILayout.Label("Part 1: Packing Input Icon Sets to Sprite Assets", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Select Icon Sets and create Sprite Assets", textStyleBold);
            showPart1 = EditorGUILayout.Foldout(showPart1, "Choose which sprites to use for displaying input actions.");
            if (showPart1)
            {

                EditorGUILayout.BeginVertical(GUI.skin.window);
                InputIconSetConfiguratorSO.Instance = (InputIconSetConfiguratorSO)EditorGUILayout.ObjectField("", InputIconSetConfiguratorSO.Instance, typeof(InputIconSetConfiguratorSO), true);
                EditorGUILayout.HelpBox("The sprites in the sets below " +
                              "will be used to create sprite assets.\n\n" +
                              "If you want to use different sprites, change them before you pack them.", MessageType.None);
                
                if(InputIconSetConfiguratorSO.Instance != null)
                {
                    EditorGUI.BeginChangeCheck();
                    DrawIconSets();
                    if(EditorGUI.EndChangeCheck())
                    {
                        InputIconsManagerSO.UpdateStyleData();
                    }
                }
                 

                EditorGUILayout.EndVertical();

                if (GUILayout.Button("Pack sets to Sprite Assets", buttonStyle))
                {
                    InputIconsLogger.Log("Packing sprites ...");
                    InputIconsSpritePacker.PackIconSets();
                    InputIconsLogger.Log("Packing sprites completed");
                }

               

            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCustomPartCopyFontAssetsAssets()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Support displaying bindings as fonts", textStyleBold);
            showCopyFontsPart = EditorGUILayout.Foldout(showCopyFontsPart, "Copy fonts");
            if (showCopyFontsPart)
            {
                EditorGUILayout.HelpBox("By pressing this button, the font assets referenced in the used Input Icon Sets " +
                   "will be copies to the resources folder of TMPro to be accessible when needed.", MessageType.None);

                if (GUILayout.Button("Copy Font Assets to Default Font folder", buttonStyle))
                {
                    CopyUsedFontAssetsToTMProDefaultFolder();
                }

                EditorGUILayout.HelpBox("To be able to display fonts, make sure the necessary styles are in the default style sheet.\n" +
                    "You can use the \"Update TMPro Style Sheet\" section below to add the styles.", MessageType.None);

            }
            EditorGUILayout.EndVertical();


        }

        private void DrawCustomPartStyleSheetUpdate()
        {
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Update TMPro Style Sheet", textStyleBold);
            showPart2 = EditorGUILayout.Foldout(showPart2, "Add or update input action asset in default style sheet");
            if (showPart2)
            {

                GUILayout.Label("Check the 'Used Input Action Assets' list above if it contains all desired Input Action Assets.", textStyle);

                if(AllInputActionAssetsNull())
                {
                    { EditorGUILayout.HelpBox("Select an Input Asset before you continue.", MessageType.Warning); }
                }
                else
                {
                    if (serializedInputActionAssets.GetArrayElementAtIndex(0) != null)
                    {
                        GUILayout.Space(10);

                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        GUILayout.Label("First prepare the default style sheet.", textStyle);

                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button("Prepare style sheet manually\n(faster, but needs you to update style sheet)"))
                        {
                            InputIconsLogger.Log("Preparing default TMP style sheet for additional entries ...");
                            managerSO.CreateInputStyleData();
                            int c = 0;
                            c += InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleKeyboardDataList);
                            c += InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleGamepadDataList);

                            InputIconsLogger.Log("TMP style sheet prepared with " + c + " empty values.");
                            if (c == 0)
                            {
                                InputIconsLogger.LogWarning(c + " empty entries added which is generally not expected. Try the same step again.");
                            }

                        }
                        GUILayout.Label("IMPORTANT: UPDATE THE STYLE SHEET.", textStyleBold);
                        GUILayout.Label("The default style sheet should now be open in the inspector. " +
                            "Make a small change in any field of the style sheet and undo it again. " +
                            "Then continue with the next step.", textStyle);

                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical();
                        GUILayout.Label("or", textStyle);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button("Prepare style sheet automatically\n(requires compilation)"))
                        {
                            InputIconsLogger.Log("Preparing default TMP style sheet for additional entries ...");
                            managerSO.CreateInputStyleData();
                            InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleKeyboardDataList);
                            InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleGamepadDataList);

                            InputIconsLogger.Log("TMP style sheet prepared.");

                            CompilationPipeline.RequestScriptCompilation();
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        DrawUILine(Color.grey);
                        GUILayout.Space(5);

                        GUILayout.Label("Now we can add/update the style sheet with our Input Action Assets.", textStyle);


                        if (!EditorApplication.isCompiling)
                        {
                            if (GUILayout.Button("Add Input Asset styles to default TMP style sheet"))
                            {
                                InputIconsLogger.Log("Adding entries default TMP style sheet for additional entries ...");
                                managerSO.CreateInputStyleData();
                                int c = 0;
                                c += InputIconsManagerSO.AddInputStyles(managerSO.inputStyleKeyboardDataList);
                                c += InputIconsManagerSO.AddInputStyles(managerSO.inputStyleGamepadDataList);

                                InputIconsLogger.Log("TMP style sheet updated with (" + c + ") styles (multiple entries combined to only one)");

                                TMP_InputStyleHack.RemoveEmptyEntriesInStyleSheet();
                            }
                        }
                        else
                        {
                            GUILayout.Label("... waiting for compilation ...", textStyleYellow);
                        }


                        GUILayout.Label("Have a look at the style sheet. You should find entries with the name of your input actions. " +
                            "The opening and closing tags might be empty but they will get filled once you start the game.", textStyle);
                        EditorGUILayout.EndVertical();
                    }
                }
               


            }
            EditorGUILayout.EndVertical();
        }

        private void DrawBottomPart()
        {

            GUILayout.Label("How to use Input Icons in TMPro text fields", textStyleHeader);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            showPart3 = EditorGUILayout.Foldout(showPart3, "Using Input Icons");
            if (showPart3)
            {

                GUILayout.Label("Once you have completed the setup you are ready to use Input Icons.", textStyle);

                GUILayout.Space(8);
                GUILayout.Label("Displaying Input Icons", textStyleBold);
                GUILayout.Label("To display Input Icons in TMPro texts, we can use the \'style\' tag.\n" +
                    "We can write <style=NameOfActionMap/NameOfAction> to display the input bindings of the action.\n" +
                    "For example we can write <style=platformer controls/move> to display the bindings of the move action " +
                    "in the platformer controls action map.\n" +
                    "If you have a \'Jump\' action, type <style=platformer controls/jump> respectively.\n" +
                    "To display a single action of a composite binding, type <style=platformer controls/move/down> for example.\n" +
                    "\n" +
                    "All available bindings are saved in the Input Icons Manager for quick access. Open the\n" +
                    "Input Icons Manager and scroll down to the lists. Copy and paste an entry of the\n" +
                    "TMPro Style Tag column into a text field to display the corresponding binding." +
                    "", textStyle);

                GUILayout.Space(8);
                GUILayout.Label("Customization", textStyleBold);
                managerSO = (InputIconsManagerSO)EditorGUILayout.ObjectField("", managerSO, typeof(InputIconsManagerSO), true);
                GUILayout.Label("The InputIconsManager provides displaying options for Input Icons and more.", textStyle);



            }
            EditorGUILayout.EndVertical();

        }

        private void DrawAdvanced()
        {
            GUILayout.Label("Advanced", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "TMP Style Sheet manipulation");
            if (showAdvanced)
            {
                EditorGUILayout.HelpBox("You can use this button to remove Input Icons style" +
                    " entries from the TMPro style sheet.", MessageType.Warning);

                var style = new GUIStyle(GUI.skin.button);

                if (GUILayout.Button("Remove all Input Icon styles from the TMPro style sheet.", style))
                {
                    InputIconsManagerSO.RemoveAllStyleSheetEntries();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawIconSets()
        {
            InputIconSetConfiguratorSO configurator = InputIconSetConfiguratorSO.Instance;
            if(configurator)
            {
                configurator.keyboardIconSet = (InputIconSetKeyboardSO)EditorGUILayout.ObjectField("Keyboard Icons", configurator.keyboardIconSet, typeof(InputIconSetKeyboardSO), true);
                configurator.ps3IconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("PS3 Icons", configurator.ps3IconSet, typeof(InputIconSetGamepadSO), true);
                configurator.ps4IconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("PS4 Icons", configurator.ps4IconSet, typeof(InputIconSetGamepadSO), true);
                configurator.ps5IconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("PS5 Icons", configurator.ps5IconSet, typeof(InputIconSetGamepadSO), true);
                configurator.switchIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("Switch Icons", configurator.switchIconSet, typeof(InputIconSetGamepadSO), true);
                configurator.xBoxIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("XBox Icons", configurator.xBoxIconSet, typeof(InputIconSetGamepadSO), true);

                configurator.fallbackGamepadIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("Fallback Icons", configurator.fallbackGamepadIconSet, typeof(InputIconSetGamepadSO), true);
                configurator.overwriteIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("Gamepads Overwrite Icons", configurator.overwriteIconSet, typeof(InputIconSetGamepadSO), true);

            }

           
            //EditorGUI.BeginDisabledGroup(true);
            /*for (int i = 0; i < iconSetSOs.Count; i++)
            {
                if(iconSetSOs[i]!=null)
                    iconSetSOs[i] = (InputIconSetBasicSO)EditorGUILayout.ObjectField(iconSetSOs[i].deviceDisplayName, iconSetSOs[i], typeof(InputIconSetBasicSO), true);
            }*/
            //EditorGUI.EndDisabledGroup();
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 5)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            //r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawUILineVertical(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding+thickness), GUILayout.ExpandHeight(true));
            r.width = thickness;
            r.x += padding / 2;
            r.y -= 2;
            r.height += 3;
            EditorGUI.DrawRect(r, color);
        }

    }
}