using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputIcons
{
    public class II_SpritePromptBehaviour : MonoBehaviour
    {

        public List<SpritePromptData> spritePromptDatas = new List<SpritePromptData>();


        private void Start()
        {
            UpdateDisplayedSprites();
            InputIconsManagerSO.onBindingsChanged += UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged += UpdateDisplayedSprites;
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedSprites;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedSprites;
        }

        public void UpdateDisplayedSprites()
        {
            foreach(SpritePromptData s in spritePromptDatas)
            {
                s.UpdateDisplayedSprite();
            }

        }
        private void UpdateDisplayedSprites(InputDevice inputDevice)
        {
            UpdateDisplayedSprites();
        }


#if UNITY_EDITOR
        public void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if (this == null) return;
            UpdateDisplayedSprites();
        }
#endif

        public class II_PromptData
        {
            public InputActionReference actionReference;
            public enum BindingSearchStrategy { BindingType, BindingIndex };
            public BindingSearchStrategy bindingSearchStrategy = BindingSearchStrategy.BindingType;
            public InputIconsUtility.BindingType bindingType = InputIconsUtility.BindingType.Up;
            public InputIconsUtility.DeviceType deviceType = InputIconsUtility.DeviceType.Auto;

            public int bindingIndex = 0;

            public Sprite GetKeySprite()
            {
                if (actionReference == null)
                    return null;

                InputIconSetBasicSO currentIconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();
                string currentControlSchemeName = currentIconSet.controlSchemeName;
                string keyName = "";

                if (bindingSearchStrategy == BindingSearchStrategy.BindingType)
                {
                    if (deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                    {
                        currentIconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                        keyName = InputIconsUtility.GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(actionReference, bindingType, false);
                    }
                    else if (deviceType == InputIconsUtility.DeviceType.Gamepad)
                    {
                        currentIconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                        keyName = InputIconsUtility.GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(actionReference, bindingType, true);
                    }
                    else
                        keyName = InputIconsUtility.GetSpriteNameOfActionForSpriteDisplay(actionReference, bindingType, currentControlSchemeName);
                }

                if (bindingSearchStrategy == BindingSearchStrategy.BindingIndex)
                {
                    if (actionReference.action.bindings[bindingIndex].groups.Contains(InputIconsManagerSO.Instance.controlSchemeName_Keyboard))
                    {
                        currentIconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                        keyName = InputIconsUtility.GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(actionReference, bindingIndex, false);
                    }
                    else
                    {
                        currentIconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                        keyName = InputIconsUtility.GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(actionReference, bindingIndex, true);
                    }
                }

                return currentIconSet.GetSprite(keyName);
            }
        }

        [System.Serializable]
        public class SpritePromptData : II_PromptData
        {
           
            public SpriteRenderer spriteRenderer;

            public void UpdateDisplayedSprite()
            {
                if (actionReference == null)
                {
                    if (spriteRenderer != null)
                        spriteRenderer.sprite = null;
                    return;
                }


                if (spriteRenderer)
                    spriteRenderer.sprite = GetKeySprite();
            }

        }
    }
}

