using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace InputIcons
{
    [CustomEditor(typeof(InputIconSetConfiguratorSO))]
    public class InputIconSetConfiguratorEditor : Editor
    {

        private ReorderableList listDeviceSets;
        private SerializedProperty disconnectedDeviceProperty;

        private void OnEnable()
        {
            disconnectedDeviceProperty = serializedObject.FindProperty("disconnectedDeviceSettings");
        }

        public override void OnInspectorGUI()
        {
            InputIconSetConfiguratorSO configuratorSO = (InputIconSetConfiguratorSO)target;
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Keyboard Icon Set", EditorStyles.boldLabel);
            configuratorSO.keyboardIconSet = (InputIconSetBasicSO)EditorGUILayout.ObjectField("Keyboard Icon Set", configuratorSO.keyboardIconSet, typeof(InputIconSetBasicSO), false);
            EditorGUILayout.LabelField("Gamepad Icon Sets", EditorStyles.boldLabel);
            configuratorSO.ps3IconSet = (InputIconSetBasicSO)EditorGUILayout.ObjectField("PS3", configuratorSO.ps3IconSet, typeof(InputIconSetBasicSO), false);
            configuratorSO.ps4IconSet = (InputIconSetBasicSO)EditorGUILayout.ObjectField("PS4", configuratorSO.ps4IconSet, typeof(InputIconSetBasicSO), false);
            configuratorSO.ps5IconSet = (InputIconSetBasicSO)EditorGUILayout.ObjectField("PS5", configuratorSO.ps5IconSet, typeof(InputIconSetBasicSO), false);
            configuratorSO.switchIconSet = (InputIconSetBasicSO)EditorGUILayout.ObjectField("Switch Icon Set", configuratorSO.switchIconSet, typeof(InputIconSetBasicSO), false);
            configuratorSO.xBoxIconSet = (InputIconSetBasicSO)EditorGUILayout.ObjectField("XBox Icon Set", configuratorSO.xBoxIconSet, typeof(InputIconSetBasicSO), false);

            EditorGUILayout.Space(7);
            EditorGUILayout.LabelField("Overwrite Gamepad Icon Set", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("If this is NOT null, only this icon set will be used to display gamepad icons.", EditorStyles.label);
            configuratorSO.overwriteIconSet = (InputIconSetBasicSO)EditorGUILayout.ObjectField(new GUIContent("Overwrite Gamepad Icon Set", "Only this will get shown if not null. Other gamepad icon sets will not be used."), configuratorSO.overwriteIconSet, typeof(InputIconSetBasicSO), false);
            EditorGUILayout.Space(7);
            EditorGUILayout.LabelField("Fallback Gamepad Icon Set", EditorStyles.boldLabel);
            configuratorSO.fallbackGamepadIconSet = (InputIconSetBasicSO)EditorGUILayout.ObjectField("Fallback Gamepad Icon Set", configuratorSO.fallbackGamepadIconSet, typeof(InputIconSetBasicSO), false);

            if(EditorGUI.EndChangeCheck())
            {
                InputIconsManagerSO.UpdateStyleData();
            }

            EditorGUILayout.Space(7);
            EditorGUILayout.PropertyField(disconnectedDeviceProperty);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(configuratorSO);
        }

    }
}