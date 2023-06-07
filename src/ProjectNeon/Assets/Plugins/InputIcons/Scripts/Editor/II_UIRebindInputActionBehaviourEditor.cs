using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.InputSystem;
using InputIcons;


[CustomEditor(typeof(II_UIRebindInputActionBehaviour))]

public class II_UIRebindInputActionBehaviourEditor : Editor
{




    private void OnEnable()
    {
        
        
    }

    public override void OnInspectorGUI()
    {

        II_UIRebindInputActionBehaviour rebindBehaviour = (II_UIRebindInputActionBehaviour)target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Binding", EditorStyles.boldLabel);
        rebindBehaviour.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", rebindBehaviour.actionReference, typeof(InputActionReference),false);
        if(InputIconsUtility.ActionIsComposite(rebindBehaviour.actionReference.action))
        {
            rebindBehaviour.bindingType = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup("Binding Type", rebindBehaviour.bindingType);
        }

        rebindBehaviour.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device", rebindBehaviour.deviceType);

        EditorGUILayout.Space(5);


        EditorGUILayout.LabelField("Rebinding", EditorStyles.boldLabel);
        rebindBehaviour.canBeRebound = EditorGUILayout.Toggle(new GUIContent("Allow Rebinding", ""), rebindBehaviour.canBeRebound);
        if(rebindBehaviour.canBeRebound)
        {
            rebindBehaviour.keyboardCancelKey = EditorGUILayout.TextField("Keyboard Cancel Key", rebindBehaviour.keyboardCancelKey);
            rebindBehaviour.gamepadCancelKey = EditorGUILayout.TextField("Gamepad Cancel Key", rebindBehaviour.gamepadCancelKey);
        }

        EditorGUILayout.Space(5);


        EditorGUILayout.LabelField("Display Texts", EditorStyles.boldLabel);
        if (rebindBehaviour.actionNameDisplayText)
        {
            rebindBehaviour.actionNameDisplayText.text = EditorGUILayout.TextField("Action Display Name", rebindBehaviour.actionNameDisplayText.text);
        }
        
        rebindBehaviour.actionNameDisplayText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Action Name Display Text", rebindBehaviour.actionNameDisplayText, typeof(TextMeshProUGUI), true);

        rebindBehaviour.bindingNameDisplayText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Binding Name Display Text", rebindBehaviour.bindingNameDisplayText, typeof(TextMeshProUGUI), true);

       
        EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
        rebindBehaviour.rebindButtonObject = (GameObject)EditorGUILayout.ObjectField("Rebind Button Object", rebindBehaviour.rebindButtonObject, typeof(GameObject), true);
        rebindBehaviour.resetButtonObject = (GameObject)EditorGUILayout.ObjectField("Reset Button Object", rebindBehaviour.resetButtonObject, typeof(GameObject), true);

        EditorGUILayout.LabelField("Display Object While Rebinding", EditorStyles.boldLabel);
        rebindBehaviour.listeningForInputObject = (GameObject)EditorGUILayout.ObjectField("Listening For Input Object", rebindBehaviour.listeningForInputObject, typeof(GameObject), true);

        EditorGUILayout.LabelField("Display Object If Binding Already Used", EditorStyles.boldLabel);
        rebindBehaviour.keyAlreadyUsedObject = (GameObject)EditorGUILayout.ObjectField("Binding Already Used Object", rebindBehaviour.keyAlreadyUsedObject, typeof(GameObject), true);

        InputIconsManagerSO manager = InputIconsManagerSO.Instance;
        manager.rebindBehaviour = (InputIconsManagerSO.RebindBehaviour)EditorGUILayout.EnumPopup(new GUIContent("Rebind Behavior",
                "Choose how to handle rebinding when the same binding already exists in the same action map."), manager.rebindBehaviour);


        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(rebindBehaviour.bindingNameDisplayText);
            EditorUtility.SetDirty(rebindBehaviour.actionNameDisplayText);
            EditorUtility.SetDirty(rebindBehaviour);
            serializedObject.ApplyModifiedProperties();

            if (Application.isEditor)
            {
                rebindBehaviour.UpdateBindingDisplay();
            }
        }
        

    }
}
