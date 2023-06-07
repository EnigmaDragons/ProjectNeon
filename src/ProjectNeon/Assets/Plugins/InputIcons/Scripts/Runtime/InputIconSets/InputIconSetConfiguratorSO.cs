using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

namespace InputIcons
{
    [CreateAssetMenu(fileName = "Input Icon Set Configurator", menuName = "Input Icon Set/InputIconSetConfigurator", order = 503)]
    public class InputIconSetConfiguratorSO : ScriptableObject
    {

        public static InputIconSetConfiguratorSO instance;
        public static InputIconSetConfiguratorSO Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                else
                {
                    InputIconSetConfiguratorSO iconManager = Resources.Load("InputIcons/InputIconSetConfigurator") as InputIconSetConfiguratorSO;
                    if (iconManager)
                        instance = iconManager;

                    return instance;
                }
            }
            set => instance = value;
        }

        private static InputIconSetBasicSO currentIconSet;
        public delegate void OnIconSetUpdated();
        public static OnIconSetUpdated onIconSetUpdated;

        public InputIconSetBasicSO keyboardIconSet;
        public InputIconSetBasicSO ps3IconSet;
        public InputIconSetBasicSO ps4IconSet;
        public InputIconSetBasicSO ps5IconSet;
        public InputIconSetBasicSO switchIconSet;
        public InputIconSetBasicSO xBoxIconSet;

        public InputIconSetBasicSO overwriteIconSet;

        public InputIconSetBasicSO fallbackGamepadIconSet;

        private InputIconSetBasicSO lastUsedGamepadIconSet;

        public DisconnectedSettings disconnectedDeviceSettings;

        private void Awake()
        {
            instance = this;
        }

        [System.Serializable]
        public struct DeviceSet
        {
            public string deviceRawPath;
            public InputIconSetBasicSO iconSetSO;
        }

        [System.Serializable]
        public struct DisconnectedSettings
        {
            public string disconnectedDisplayName;
            public Color disconnectedDisplayColor;
        }

        public static void UpdateCurrentIconSet(InputDevice device)
        {
            currentIconSet = GetIconSet(device);
            SetCurrentIconSet(currentIconSet);
            onIconSetUpdated?.Invoke();
        }

        public static void SetCurrentIconSet(InputIconSetBasicSO iconSet)
        {
            if (iconSet == null)
                return;

            currentIconSet = iconSet;

            if(iconSet.GetType() == typeof(InputIconSetGamepadSO))
            {
                Instance.lastUsedGamepadIconSet = iconSet;
            }
        }

        public static InputIconSetBasicSO GetCurrentIconSet()
        {
            if (currentIconSet == null) UpdateCurrentIconSet(InputIconsManagerSO.GetCurrentInputDevice());


            return currentIconSet;
        }

        public static InputIconSetBasicSO GetLastUsedGamepadIconSet()
        {
            if (Instance.lastUsedGamepadIconSet == null)
                return Instance.fallbackGamepadIconSet;

            return Instance.lastUsedGamepadIconSet;
        }

        public static List<InputIconSetBasicSO> GetAllIconSetsOnConfigurator()
        {
            List<InputIconSetBasicSO> sets = new List<InputIconSetBasicSO>();

            InputIconSetConfiguratorSO configurator = Instance;
            if(configurator)
            {
                sets.Add(configurator.keyboardIconSet);
                sets.Add(configurator.ps3IconSet);
                sets.Add(configurator.ps4IconSet);
                sets.Add(configurator.ps5IconSet);
                sets.Add(configurator.switchIconSet);
                sets.Add(configurator.xBoxIconSet);

                sets.Add(configurator.overwriteIconSet);
                sets.Add(configurator.fallbackGamepadIconSet);
            }

            return sets;
        }

        public static InputIconSetBasicSO GetIconSet(InputDevice device)
        {

            if (device == null)
                return Instance.keyboardIconSet;


            //Debug.Log(activeDevice.displayName);
            
            if(device is Gamepad)
            {
                if (Instance.overwriteIconSet != null) //if overwriteIconSet is not null, this set will be used for all gamepads
                    return Instance.overwriteIconSet;

                if (device is UnityEngine.InputSystem.XInput.XInputController)
                {
                    return Instance.xBoxIconSet;
                }

#if !UNITY_STANDALONE_LINUX && !UNITY_WEBGL
                if (device is UnityEngine.InputSystem.DualShock.DualShock3GamepadHID)
                {
                    return Instance.ps3IconSet;
                }

                if (device is UnityEngine.InputSystem.DualShock.DualShock4GamepadHID)
                {
                    return Instance.ps4IconSet;
                }

                if (device is UnityEngine.InputSystem.DualShock.DualSenseGamepadHID) //Input System 1.2.0 or higher required (package manager dropdown menu -> see other versions)
                {
                    return Instance.ps5IconSet;
                }

                if (device is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
                {
                    return Instance.switchIconSet;
                }
#endif

                if (device is UnityEngine.InputSystem.DualShock.DualShockGamepad)
                {
                    return Instance.ps4IconSet;
                }

                if (device.name.Contains("DualShock3"))
                    return Instance.ps3IconSet;

                if (device.name.Contains("DualShock4"))
                    return Instance.ps4IconSet;

                if (device.name.Contains("DualSense"))
                    return Instance.ps5IconSet;

                if (device.name.Contains("ProController"))
                    return Instance.switchIconSet;
            }
           

            //in case it is none of the above gamepads, return fallback icons
            if(device is Gamepad)
            {
                return Instance.fallbackGamepadIconSet;
            }

            return Instance.keyboardIconSet;
        }

        public static InputIconSetBasicSO GetIconSet(string iconSetName)
        {
            List<InputIconSetBasicSO> sets = GetAllIconSetsOnConfigurator();
            for(int i=0; i<sets.Count; i++)
            {
                if (sets[i] == null)
                    continue;

                if(sets[i].iconSetName == iconSetName)
                    return sets[i];
            }

            InputIconsLogger.LogWarning("Icon Set not found: " + iconSetName);
            return null;
        }

        public static string GetCurrentDeviceName()
        {
            return GetCurrentIconSet().iconSetName;
        }

        public static Color GetCurrentDeviceColor()
        {
            return GetCurrentIconSet().deviceDisplayColor;
        }

        public static string GetDisconnectedName()
        {
            return Instance.disconnectedDeviceSettings.disconnectedDisplayName;
        }

        public static Color GetDisconnectedColor()
        {
            return Instance.disconnectedDeviceSettings.disconnectedDisplayColor;
        }


    }
}