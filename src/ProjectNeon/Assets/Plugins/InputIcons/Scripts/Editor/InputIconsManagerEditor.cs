using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace InputIcons
{
    [CustomEditor(typeof(InputIconsManagerSO))]
    public class InputIconsManagerEditor : Editor
    {
        private ReorderableList actionNameRenamings;
        private ReorderableList styleTagKeyboardDatas;
        private ReorderableList styleTagGamepadDatas;

        private void OnEnable()
        {
            DrawCustomContextList();
        }

        private void UpdateManagerValues()
        {
            InputIconsManagerSO.UpdateStyleData();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            InputIconsManagerSO iconsManager = (InputIconsManagerSO)target;

            if (GUILayout.Button("Open Setup Window"))
            {
                InputIconsSetupWindow.ShowWindow();
            }

            EditorGUI.BeginChangeCheck();


            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Setup Settings", EditorStyles.boldLabel);

            var inputList = serializedObject.FindProperty("usedActionAssets");
            EditorGUILayout.PropertyField(inputList, new GUIContent("Used Input Action Assets"), true);

            if (EditorGUI.EndChangeCheck())
            {
                UpdateManagerValues();
            }

            EditorGUILayout.LabelField("Input Action Asset Schemes", EditorStyles.boldLabel);
            iconsManager.controlSchemeName_Keyboard = EditorGUILayout.TextField(new GUIContent("Keyboard Control Scheme Name",
                "The name of the control scheme you use for Keyboard and Mouse in the Input Action Asset"), iconsManager.controlSchemeName_Keyboard);
            iconsManager.controlSchemeName_Gamepad = EditorGUILayout.TextField(new GUIContent("Gamepad Control Scheme Name",
                "The name of the control scheme you use for Gamepad control in the Input Action Asset"), iconsManager.controlSchemeName_Gamepad);

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);




            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Default Sprites To Show On Start", EditorStyles.boldLabel);
            iconsManager.defaultStartDeviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup(new GUIContent("Peferred Start Device",
                "Choose which device should be displayed when the game starts. The manager will try to automatically display the preferred type. " +
                "Note, if this is set to gamepad, gamepad icons will only be displayed if a gamepad is connected and available - same for keyboard."),
                iconsManager.defaultStartDeviceType);

            EditorGUILayout.LabelField("Gamepad Settings", EditorStyles.boldLabel);
            iconsManager.gamepadIconDisplaySetting = (InputIconsManagerSO.GamepadIconDisplaySetting)EditorGUILayout.EnumPopup(new GUIContent("Gamepad priority",
                "Choose which gamepad should be displayed.\n" +
                "Last Used: The currently used gamepad.\n" +
                "First Connected: The gamepad which was detected first.\n" +
                "Note: For Steam games this will always behave like be First Connected."),
                iconsManager.gamepadIconDisplaySetting);

            GUILayout.Space(15);
            EditorGUILayout.LabelField("Update Settings", EditorStyles.boldLabel);
            iconsManager.textUpdateOptions = (InputIconsManagerSO.TextUpdateOptions)EditorGUILayout.EnumPopup(new GUIContent("Text Update Method",
                "Choose how to update texts on device change.\n\n" +
                "'Search and Update' will search for all text objects in the scene on device change - slower but reliable\n\n" +
                "'Via InputIconsText Components' requires you to add a InputIconsText component to each text that should be updated - much more performant but you need to remember to add the required component"),
                iconsManager.textUpdateOptions);
            iconsManager.deviceChangeDelay = EditorGUILayout.FloatField(new GUIContent("Update Delay",
                "Add a short delay when changing devices to ensure user actually intended to use a different device " +
                "and did not accidentally hit a button on a controller. This improves performance and also prevents icons from constantly switching " +
                "if a device constantly sends signals."), iconsManager.deviceChangeDelay);

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
          
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Display Settings", EditorStyles.boldLabel);

            iconsManager.displayType = (InputIconsManagerSO.DisplayType)EditorGUILayout.EnumPopup(new GUIContent("Display Type",
    "Choose how to display input action bindings. Sprites, Text or [Text]"), iconsManager.displayType);

            if(iconsManager.displayType == InputIconsManagerSO.DisplayType.Sprites)
            {
                iconsManager.tintingEnabled = EditorGUILayout.Toggle(new GUIContent("Allow Tinting",
                    "Enable this to allow icons to be tinted by text color or via the color rich text tag."), iconsManager.tintingEnabled);
            }

            iconsManager.showAllInputOptionsInStyles = EditorGUILayout.Toggle(new GUIContent("Show All Available Input Options",
                "Enable this to show WASD and Arrow Keys for movement control for example."), iconsManager.showAllInputOptionsInStyles);
            if(iconsManager.showAllInputOptionsInStyles)
            {
                iconsManager.multipleInputsDelimiter = EditorGUILayout.TextField(new GUIContent("Multiple Input Delimiter",
                    "This will be used as a delimiter in case there is more than one binding for an action."), iconsManager.multipleInputsDelimiter);
            }

            iconsManager.openingTag = EditorGUILayout.TextField(new GUIContent("Opening Tag",
               "Can be used to apply additional styling to displayed sprites. E.g. <size=120%>"), iconsManager.openingTag);
            iconsManager.closingTag = EditorGUILayout.TextField(new GUIContent("Closing Tag",
               "Needed to close tags in the opening tag field. E.g. </size>"), iconsManager.closingTag);


            if(iconsManager.displayType == InputIconsManagerSO.DisplayType.Text
                || iconsManager.displayType == InputIconsManagerSO.DisplayType.TextInBrackets)
            {
                iconsManager.compositeInputDelimiter = EditorGUILayout.TextField(new GUIContent("Composite Input Delimiter",
                    "This will be used as a delimiter for composite bindings. " +
                    "E.g. using ', ' as a delimiter will turn WASD into W, A, S, D"), iconsManager.compositeInputDelimiter);

                GUILayout.Space(5);

                EditorGUILayout.LabelField("Text Display Customization", EditorStyles.boldLabel);
                iconsManager.textDisplayForUnboundActions = EditorGUILayout.TextField(new GUIContent("Text Display For Unbound Actions",
                   "If an action does not have a binding, display this instead"), iconsManager.textDisplayForUnboundActions);

                iconsManager.textDisplayLanguage = (InputIconsManagerSO.TextDisplayLanguage)EditorGUILayout.EnumPopup(new GUIContent("Text Display Language",
                 "Choose which language should be used when displaying Input Bindings as text."), iconsManager.textDisplayLanguage);
            }

           

            if (EditorGUI.EndChangeCheck())
            {
                UpdateManagerValues();
            }

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Action name output overrides", EditorStyles.boldLabel);
            actionNameRenamings.DoLayoutList();

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Logging", EditorStyles.boldLabel);
            InputIconsManagerSO.Instance.loggingEnabled = EditorGUILayout.Toggle("Logging enabled", InputIconsManagerSO.Instance.loggingEnabled);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);

          
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Rebinding Options", EditorStyles.boldLabel);

            iconsManager.rebindBehaviour = (InputIconsManagerSO.RebindBehaviour)EditorGUILayout.EnumPopup(new GUIContent("Rebind Behaviour",
                "Choose how to handle rebinding when the same binding already exists in the same action map."), iconsManager.rebindBehaviour);

            iconsManager.loadAndSaveInputBindingOverrides = EditorGUILayout.Toggle(new GUIContent("Load And Save Input Binding Overrides",
                "Enable this to load any saved changes made to the bindings of the Input Action Asset."), iconsManager.loadAndSaveInputBindingOverrides);
            EditorGUILayout.EndVertical();

            GUILayout.Space(15);
            EditorGUILayout.BeginVertical(GUI.skin.box);

            if (GUILayout.Button("Create Data"))
            {
                iconsManager.CreateInputStyleData();

            }

            EditorGUILayout.LabelField("List of keyboard input data. Automatically updated at runtime when needed.", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Copy TMPro Style Tag entry into a textfield to display bindings.", EditorStyles.label);
            styleTagKeyboardDatas.DoLayoutList();
            styleTagKeyboardDatas.displayAdd = false;
            styleTagKeyboardDatas.displayRemove = false;
            styleTagKeyboardDatas.draggable = false;

            EditorGUILayout.LabelField("List of gamepad input data. Automatically updated at runtime when needed.", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Copy TMPro Style Tag entry into a textfield to display bindings.", EditorStyles.label);
            styleTagGamepadDatas.DoLayoutList();
            styleTagGamepadDatas.displayAdd = false;
            styleTagGamepadDatas.displayRemove = false;
            styleTagGamepadDatas.draggable = false;

            EditorGUILayout.EndVertical();


            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(iconsManager);
        }


        void DrawCustomContextList()
        {
            try
            {
                actionNameRenamings = new ReorderableList(serializedObject, serializedObject.FindProperty("actionNameRenamings"), true, true, true, true);

                actionNameRenamings.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, 140, EditorGUIUtility.singleLineHeight), "From Action Name");
                    EditorGUI.LabelField(new Rect(rect.x + 175, rect.y, 100, EditorGUIUtility.singleLineHeight), "To New Name");
                };

                actionNameRenamings.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {

                    var element = actionNameRenamings.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x + 5, rect.y, 140, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("originalString"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 160, rect.y, 170, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("outputString"), GUIContent.none);
                };
            }
            catch(System.Exception)
            {
                //SerializedObjectNotCreatableException might appear on older Unity Versions. Not critical
            }

            try
            {
                styleTagKeyboardDatas = new ReorderableList(serializedObject, serializedObject.FindProperty("inputStyleKeyboardDataList"), true, true, true, true);

                styleTagKeyboardDatas.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, 100, EditorGUIUtility.singleLineHeight), "TMPro Style Tag");
                    EditorGUI.LabelField(new Rect(rect.x + 110, rect.y, 140, EditorGUIUtility.singleLineHeight), "TMPro Font Style Tag");
                    EditorGUI.LabelField(new Rect(rect.x + 260, rect.y, 140, EditorGUIUtility.singleLineHeight), "Binding");
                    EditorGUI.LabelField(new Rect(rect.x + 460, rect.y, 200, EditorGUIUtility.singleLineHeight), "Single Opening Tag - Single");
                    EditorGUI.LabelField(new Rect(rect.x + 660, rect.y, 200, EditorGUIUtility.singleLineHeight), "Font Code");
                    //EditorGUI.LabelField(new Rect(rect.x + 660, rect.y, 200, EditorGUIUtility.singleLineHeight), "Style Opening Tag - All");
                };

                styleTagKeyboardDatas.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {

                    var element = styleTagKeyboardDatas.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x + 5, rect.y, 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("tmproReferenceText"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y, 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fontReferenceText"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 240, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("bindingName"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 450, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("inputStyleString_singleInput"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 670, rect.y, 1430, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fontCode"), GUIContent.none);
                    //EditorGUI.PropertyField(new Rect(rect.x + 670, rect.y, 1430, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("inputStyleString"), GUIContent.none);
                };




                styleTagGamepadDatas = new ReorderableList(serializedObject, serializedObject.FindProperty("inputStyleGamepadDataList"), true, true, true, true);

                styleTagGamepadDatas.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, 100, EditorGUIUtility.singleLineHeight), "TMPro Style Tag");
                    EditorGUI.LabelField(new Rect(rect.x + 110, rect.y, 140, EditorGUIUtility.singleLineHeight), "TMPro Font Style Tag");
                    EditorGUI.LabelField(new Rect(rect.x + 260, rect.y, 140, EditorGUIUtility.singleLineHeight), "Binding");
                    EditorGUI.LabelField(new Rect(rect.x + 460, rect.y, 200, EditorGUIUtility.singleLineHeight), "Single Opening Tag - Single");
                    EditorGUI.LabelField(new Rect(rect.x + 660, rect.y, 200, EditorGUIUtility.singleLineHeight), "Font Code");
                    //EditorGUI.LabelField(new Rect(rect.x + 660, rect.y, 200, EditorGUIUtility.singleLineHeight), "Style Opening Tag - All");
                };

                styleTagGamepadDatas.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {

                    var element = styleTagGamepadDatas.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x + 5, rect.y, 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("tmproReferenceText"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y, 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fontReferenceText"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 240, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("bindingName"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 450, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("inputStyleString_singleInput"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 670, rect.y, 1430, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fontCode"), GUIContent.none);
                    //EditorGUI.PropertyField(new Rect(rect.x + 670, rect.y, 1430, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("inputStyleString"), GUIContent.none);
                };
            }
            catch(System.Exception)
            {

            }
        }
    }
}
