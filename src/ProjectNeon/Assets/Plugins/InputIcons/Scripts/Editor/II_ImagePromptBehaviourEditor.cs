using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InputIcons.II_ImagePromptBehaviour;

namespace InputIcons
{
    [CustomEditor(typeof(II_ImagePromptBehaviour))]
    public class II_ImagePromptBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            II_ImagePromptBehaviour imagePromptBehaviour = (II_ImagePromptBehaviour)target;

            EditorGUI.BeginChangeCheck();

            foreach (II_ImagePromptBehaviour.ImagePromptData sData in imagePromptBehaviour.spritePromptDatas.ToList())
            {
                sData.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", sData.actionReference, typeof(InputActionReference), false);

                if (sData.actionReference != null)
                {
                    sData.bindingSearchStrategy = (ImagePromptData.BindingSearchStrategy)EditorGUILayout.EnumPopup("Search Binding By", sData.bindingSearchStrategy);
                    if (sData.bindingSearchStrategy == ImagePromptData.BindingSearchStrategy.BindingType)
                    {
                        if (InputIconsUtility.ActionIsComposite(sData.actionReference.action))
                        {
                            sData.bindingType = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup("Binding Type", sData.bindingType);
                        }
                        sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);
                    }
                    else if (sData.bindingSearchStrategy == ImagePromptData.BindingSearchStrategy.BindingIndex)
                    {
                        sData.bindingIndex = EditorGUILayout.IntSlider("Binding Index", sData.bindingIndex, 0, sData.actionReference.action.bindings.Count - 1);
                    }
                }

                sData.image = (Image)EditorGUILayout.ObjectField("Display Image", sData.image, typeof(Image), true);

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                {
                    imagePromptBehaviour.spritePromptDatas.Remove(sData);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(20);
            }


            if (GUILayout.Button("Add"))
            {
                imagePromptBehaviour.spritePromptDatas.Add(new ImagePromptData());
            }

            if (EditorGUI.EndChangeCheck())
            {
                imagePromptBehaviour.OnValidate();
            }

        }
    }

}
