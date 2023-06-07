using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputIcons.II_SpritePromptBehaviour;

namespace InputIcons
{
    [CustomEditor(typeof(II_SpritePromptBehaviour))]
    public class II_SpritePromptBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            II_SpritePromptBehaviour spritePromptBehaviour = (II_SpritePromptBehaviour)target;

            EditorGUI.BeginChangeCheck();
            
            foreach(SpritePromptData sData in spritePromptBehaviour.spritePromptDatas.ToList())
            {
                sData.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", sData.actionReference, typeof(InputActionReference), false);
                if (sData.actionReference != null)
                {
                    sData.bindingSearchStrategy = (SpritePromptData.BindingSearchStrategy)EditorGUILayout.EnumPopup("Search Binding By", sData.bindingSearchStrategy);
                    if (sData.bindingSearchStrategy == SpritePromptData.BindingSearchStrategy.BindingType)
                    {

                        if (InputIconsUtility.ActionIsComposite(sData.actionReference.action))
                        {
                            sData.bindingType = (InputIconsUtility.BindingType)EditorGUILayout.EnumPopup("Binding Type", sData.bindingType);
                        }

                        sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);
                    }
                    else if (sData.bindingSearchStrategy == SpritePromptData.BindingSearchStrategy.BindingIndex)
                    {
                        sData.bindingIndex = EditorGUILayout.IntSlider("Binding Index", sData.bindingIndex, 0, sData.actionReference.action.bindings.Count - 1);
                    }
                }

                sData.spriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Sprite Renderer", sData.spriteRenderer, typeof(SpriteRenderer), true);

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                {
                    spritePromptBehaviour.spritePromptDatas.Remove(sData);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(20);
            }


            if (GUILayout.Button("Add"))
            {
                spritePromptBehaviour.spritePromptDatas.Add(new SpritePromptData());
            }

            if (EditorGUI.EndChangeCheck())
            {
                spritePromptBehaviour.OnValidate();
            }

        }
    }

}
