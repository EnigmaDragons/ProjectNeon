using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace InputIcons
{
    [CustomEditor(typeof(InputIconSetKeyboardSO))]
    public class InputIconSetKeyboardEditor : Editor
    {
        private ReorderableList customInputContextIconList;
        private static List<KeyValuePair<string, string>> savedFontCodes = new List<KeyValuePair<string, string>>();

        private void OnEnable()
        {
            DrawCustomContextList();
        }

        public override void OnInspectorGUI()
        {
            InputIconSetKeyboardSO iconSet = (InputIconSetKeyboardSO)target;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            iconSet.iconSetName = EditorGUILayout.TextField("Icon Set Name", iconSet.iconSetName);
            iconSet.deviceDisplayColor = EditorGUILayout.ColorField("Device Display Color", iconSet.deviceDisplayColor);


            EditorGUILayout.Space(5);

            if (GUILayout.Button("Pack to Sprite Asset", GUILayout.Height(30)))
            {
                InputIconsSpritePacker.PackIconSet(iconSet);
                return;
            }
            EditorGUILayout.Space(5);

            if (GUILayout.Button("Initialize Mouse Buttons", GUILayout.Height(30)))
            {
                iconSet.InitializeMouseButtons();
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("To change the used sprites more conveniently, drag this icon set into a folder containing sprites. " +
                "Then click the button below to try to automatically find corresponding sprites." +
                "Check if all sprites were found. \nNote: Custom context icons must be assigned manually. \nThen use the \"Pack to Sprite Asset\" button above.", MessageType.Info);
            if (GUILayout.Button("Automatically apply button sprites of folder", GUILayout.Height(30)))
            {
                Undo.RecordObject(iconSet, "Apply Sprites Of Folder");
                iconSet.TryGrabSprites();
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.HelpBox("Use the below buttons to copy and paste the Font Codes from one Icon Set to another. The sets must be of the same type for this to work properly (keyboard or gamepad).", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy font codes", GUILayout.Height(30)))
            {
                List<InputSpriteData> data = iconSet.GetAllInputSpriteData();
                savedFontCodes = new List<KeyValuePair<string, string>>();
                foreach (InputSpriteData dataItem in data)
                {
                    savedFontCodes.Add(new KeyValuePair<string, string>(dataItem.textMeshStyleTag, dataItem.fontCode));
                }
                 
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Paste font codes", GUILayout.Height(30)))
            {
                Undo.RecordObject(iconSet, "Paste font codes: "+iconSet.iconSetName);
                iconSet.ApplyFontCodes(savedFontCodes);
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            iconSet.fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("Font Asset", iconSet.fontAsset, typeof(TMP_FontAsset), false);

            iconSet.unboundData = DrawDeviceField(iconSet.unboundData);
            iconSet.fallbackData = DrawDeviceField(iconSet.fallbackData);

            EditorGUILayout.LabelField("Icons - Custom Contexts", EditorStyles.boldLabel);
            customInputContextIconList.DoLayoutList();

            EditorGUILayout.LabelField("Icons - Mouse", EditorStyles.boldLabel);

            iconSet.mouse = DrawDeviceField(iconSet.mouse);
            iconSet.mouse_left = DrawDeviceField(iconSet.mouse_left);
            iconSet.mouse_right = DrawDeviceField(iconSet.mouse_right);
            iconSet.mouse_middle = DrawDeviceField(iconSet.mouse_middle);

            EditorGUILayout.LabelField("Icons - Keys", EditorStyles.boldLabel);

            for (int i = 0; i < iconSet.inputKeys.Count; i++)
            {
                iconSet.inputKeys[i] = DrawDeviceField(iconSet.inputKeys[i]);
            }

            if(EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(iconSet);
            }

        }

        private InputSpriteData DrawDeviceField(InputSpriteData data)
        {

            EditorGUILayout.LabelField(data.GetButtonName());

            EditorGUILayout.BeginHorizontal(GUILayout.Width(200));

            EditorGUIUtility.labelWidth = 40;
            EditorGUIUtility.fieldWidth = 50;
            data.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", data.sprite, typeof(Sprite), false);
            EditorGUILayout.Space(10);
            EditorGUIUtility.labelWidth = 60;
            data.fontCode = EditorGUILayout.TextField("FontCode", data.fontCode);
            EditorGUILayout.Space(10);
            GUI.enabled = false;
            EditorGUIUtility.labelWidth = 110;
            EditorGUIUtility.fieldWidth = 80;
            data.textMeshStyleTag = EditorGUILayout.TextField("TextMeshStyleTag", data.textMeshStyleTag);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            InputSpriteData d = new InputSpriteData(data.GetButtonName(), data.sprite, data.textMeshStyleTag, data.fontCode);

            return d;
        }

        Rect CalculateColumn(Rect rect, int columnNumber, float xPadding, float xWidth)
        {
            float xPosition = rect.x;
            switch (columnNumber)
            {
                case 1:
                    xPosition = rect.x + xPadding;
                    break;

                case 2:
                    xPosition = rect.x + rect.width / 2 + xPadding;
                    break;
            }


            return new Rect(xPosition, rect.y, rect.width / 2 - xWidth, EditorGUIUtility.singleLineHeight);
        }

        void DrawCustomContextList()
        {
            customInputContextIconList = new ReorderableList(serializedObject, serializedObject.FindProperty("customContextIcons"), true, true, true, true);

            customInputContextIconList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, 140, EditorGUIUtility.singleLineHeight), "Input Binding String");
                EditorGUI.LabelField(new Rect(rect.x + 140, rect.y, 100, EditorGUIUtility.singleLineHeight), "Font Code");
                EditorGUI.LabelField(new Rect(rect.x + 215, rect.y, 100, EditorGUIUtility.singleLineHeight), "Display Icon");
            };

            customInputContextIconList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {

                var element = customInputContextIconList.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 2;

                EditorGUI.PropertyField(new Rect(rect.x + 5, rect.y, 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("textMeshStyleTag"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 130, rect.y, 50, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fontCode"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 200, rect.y, 170, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("customInputContextSprite"), GUIContent.none);


            };
        }
    }
}