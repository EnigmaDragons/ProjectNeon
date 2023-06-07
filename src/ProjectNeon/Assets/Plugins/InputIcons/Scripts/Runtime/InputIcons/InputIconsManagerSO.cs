using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static InputIcons.InputIconsUtility;

namespace InputIcons
{

    [CreateAssetMenu(fileName = "InputIconsManager", menuName = "Input Icon Set/Input Icons Manager", order = 504)]
    public class InputIconsManagerSO : ScriptableObject
    {
        private static InputIconsManagerSO instance;
        public static InputIconsManagerSO Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                else
                {
                    InputIconsManagerSO iconManager = Resources.Load("InputIcons/InputIconsManager") as InputIconsManagerSO;
                    if (iconManager)
                    {
                        instance = iconManager;
                    }
                    

                    return instance;
                }
            }
            set => instance = value;
        }

        public InputIconSetConfiguratorSO iconSetConfiguratorSO;

        private InputDevice currentInputDevice;
        private static InputDevice inputDeviceToChangeTo;

        public List<InputActionAsset> usedActionAssets;

        [Tooltip("The name of the keyboard control scheme in the Input Action Asset")]
        public string controlSchemeName_Keyboard = "Keyboard And Mouse";
        [Tooltip("The name of the gamepad control scheme in the Input Action Asset")]
        public string controlSchemeName_Gamepad = "Gamepad";

        [Header("Display Options")]
        [Tooltip("If true, will display 'WASD or Arrowkeys' in <style=Move> for example. If false, will only display WASD (or the first option set in the Input Action Asset).")]
        public bool showAllInputOptionsInStyles = false;
        public bool tintingEnabled = false;
        public string openingTag = "";
        public string closingTag = "";
        public string multipleInputsDelimiter = " <size=80%>or</size> ";
        public string compositeInputDelimiter = ", ";

        public string textDisplayForUnboundActions = "Undefined";
        public enum TextDisplayLanguage { EnglishOnly, SystemLanguage };
        public TextDisplayLanguage textDisplayLanguage = TextDisplayLanguage.EnglishOnly;

        public List<ActionRenamingStruct> actionNameRenamings = new List<ActionRenamingStruct>();
        [System.Serializable]
        public struct ActionRenamingStruct
        {
            public string originalString;
            public string outputString;
        }

        public InputIconsUtility.DeviceType defaultStartDeviceType = InputIconsUtility.DeviceType.KeyboardAndMouse;

        public enum GamepadIconDisplaySetting {LastUsed, FirstConnected};
        public GamepadIconDisplaySetting gamepadIconDisplaySetting = GamepadIconDisplaySetting.LastUsed;

        private Gamepad firstConnectedGamepad;
        private static DateTime sceneLoadedTime;

        public enum TextUpdateOptions { SearchAndUpdate, ViaInputIconsTextComponents };
        [Header("Text update options")]
        public TextUpdateOptions textUpdateOptions = TextUpdateOptions.SearchAndUpdate;
        private static readonly List<InputIconsText> activeTexts = new List<InputIconsText>();

        public enum RebindBehaviour { OverrideExisting, CancelOverrideIfBindingAlreadyExists };
        [Header("Rebinding Options")]
        public RebindBehaviour rebindBehaviour = RebindBehaviour.OverrideExisting;
        public bool loadAndSaveInputBindingOverrides = true;

        public enum DisplayType { Sprites, Text, TextInBrackets };
        public DisplayType displayType = DisplayType.Sprites;

        public delegate void OnControlsChanged(InputDevice inputDevice);
        public static OnControlsChanged onControlsChanged;

        public delegate void OnBindingsChanged();
        public static OnBindingsChanged onBindingsChanged;

        private static II_ShowDeviceIconsAfterDelay deviceDisplayChanger;
        public float deviceChangeDelay = 0.3f;

        public static event Action onStylesPrepared;
        public static event Action onStylesAddedToStyleSheet;
        
        public static event Action onBindingsReset;
        public static event Action onStyleSheetsUpdated;


        public List<InputStyleData> inputStyleKeyboardDataList;
        public List<InputStyleData> inputStyleGamepadDataList;

        private string lastKeyboardLayout = "";
        private InputIconSetBasicSO lastGamepadIconSet = null;

        public bool isUsingFonts = false;

        public string TEXTMESHPRO_SPRITEASSET_FOLDERPATH = "Assets/TextMesh Pro/Resources/Sprite Assets/";
        public string TEXTMESHPRO_FONTASSET_FOLDERPATH = "Assets/TextMesh Pro/Resources/Fonts & Materials/";

        public bool loggingEnabled = true;

        private static bool listeningForDeviceChange = false;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RunOnStart()
        {
            if (UnityEditor.EditorSettings.enterPlayModeOptionsEnabled)
            {
                Instance.Initialize();
            }
        }
#endif

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Instance = this;
            Instance.Initialize();

#if STEAMWORKS_NET
            ScriptableObject.CreateInstance<InputIconsSteamworksExtensionSO>();
#endif

        }

        public void Initialize()
        {
            LoadUserRebinds(); //See if we have saved binding overrides and reapply them

            //Save the first connected gamepad in case we only want to display the first one at some point
            List<Gamepad> gamepads = new List<Gamepad>(Gamepad.all);
            foreach (Gamepad gamepad in gamepads)
            {
                if (gamepad != null && firstConnectedGamepad == null)
                {
                    firstConnectedGamepad = gamepad;
                    //InputIconsLogger.Log("First gamepad connected: " + firstConnectedGamepad.displayName);
                }
            }
            InputSystem.onDeviceChange += UpdateFirstConnectedGamepad; //keep the first connected gamepad updated

            StartUpdatingIconsBehaviour(); //react to device changes

            if (Application.isPlaying)
            {
                ShowDeviceIconsBasedOnSettings();
            }
            //InputIconsLogger.Log("listening for on device change event");
            
        }

        //Keep the first connected gamepad updated
        private void UpdateFirstConnectedGamepad(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added && device is Gamepad)
            {
                if (firstConnectedGamepad == null)
                {
                    firstConnectedGamepad = (Gamepad)device;
                    InputIconsLogger.Log("First gamepad connected: " + firstConnectedGamepad.displayName);
                }
            }
            else if (change == InputDeviceChange.Removed && device == firstConnectedGamepad)
            {
                // The first connected gamepad was disconnected, find the next one
                firstConnectedGamepad = null;
                var gamepads = Gamepad.all;
                foreach (Gamepad gamepad in gamepads)
                {
                    if (gamepad != null)
                    {
                        firstConnectedGamepad = gamepad;
                        InputIconsLogger.Log("Next gamepad connected: " + firstConnectedGamepad.displayName);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Start listening for device changes and automatically update icons accordingly.
        /// </summary>
        public static void StartUpdatingIconsBehaviour()
        {
            if (listeningForDeviceChange) //do not subscribe multiple times
                return;

            SceneManager.sceneLoaded += HandleSceneLoaded;
            InputSystem.onActionChange += HandleDeviceChange;
            listeningForDeviceChange = true;
        }

        /// <summary>
        /// Use to turn off automatic Icon Updating
        /// </summary>
        public static void StopUpdatingIconsBehaviour()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            InputSystem.onActionChange -= HandleDeviceChange;
            listeningForDeviceChange = false;
        }

        private static void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (Application.isPlaying)
            {
                //Debug.Log("scene loaded, show current device: " + GetCurrentInputDevice().name);
                sceneLoadedTime = DateTime.Now;
                SetDeviceAndRefreshDisplayedIcons(GetCurrentInputDevice());
            }
        }

        public static void ShowDeviceIconsBasedOnSettings()
        {
            InputIconsUtility.DeviceType selectedDeviceType = Instance.defaultStartDeviceType;
            if (selectedDeviceType != InputIconsUtility.DeviceType.Auto)
            {
                InputIconsLogger.Log("show preferred device type ..");
                if (Application.isPlaying)
                {
                    GameObject helperObject = new GameObject("InputIconsSpriteChanger");
                    II_ShowDeviceIconsAfterDelay deviceDisplayer = helperObject.AddComponent<II_ShowDeviceIconsAfterDelay>();
                    if (selectedDeviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                        deviceDisplayer.ShowKeyboardIconsAfterDelay();

                    if (selectedDeviceType == InputIconsUtility.DeviceType.Gamepad)
                        deviceDisplayer.ShowGamepadIconsAfterDelay();

                    deviceDisplayer.DestroyAfterDelay(0.5f);
                }
                else
                {
                    if (selectedDeviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                        ShowKeyboardIconsIfKeyboardAvailable();

                    if (selectedDeviceType == InputIconsUtility.DeviceType.Gamepad)
                        ShowGamepadIconsIfGamepadAvailable();
                }
            }
        }

        public static void ShowGamepadIconsIfGamepadAvailable()
        {
            if (Gamepad.current != null)
            {
                SetCurrentInputDevice(Gamepad.current);
                RefreshCurrentInputDeviceSprites();
            }
        }

        public static void ShowKeyboardIconsIfKeyboardAvailable()
        {
            if (Keyboard.current != null)
            {
                SetCurrentInputDevice(Keyboard.current);
                RefreshCurrentInputDeviceSprites();
            }
        }

        public static void RefreshCurrentInputDeviceSprites()
        {
            InputIconSetConfiguratorSO.UpdateCurrentIconSet(GetCurrentInputDevice());
            Instance.UpdateInputStyleData();
            InputIconsUtility.RefreshAllTMProUGUIObjects();
            onControlsChanged?.Invoke(GetCurrentInputDevice());
        }

        public static void HandleInputBindingsChanged()
        {
            Instance.CreateInputStyleData();
            onBindingsChanged?.Invoke();

            InputIconsUtility.RefreshAllTMProUGUIObjects();
        }

        public static bool DeviceIsKeyboardOrMouse(InputDevice device)
        {
            if (device == null)
                return false;

            if (device is Keyboard || device is Mouse)
                return true;

            return false;
        }

        public static void HandleDeviceChange(object obj, InputActionChange change)
        {
            //after a scene load, the PlayerInput component triggers the onActionChange event with a mouse
            //check if a scene was recently loaded and do not update to avoid changing icons to keyboard
            //when the user is actually using a gamepad
            if (sceneLoadedTime != null)
                if ((DateTime.Now - sceneLoadedTime).TotalSeconds < 0.2f) 
                    return;

            if (change != InputActionChange.ActionPerformed)
                return;

            InputDevice device = ((InputAction)obj).activeControl.device;

            if (device == null)
                return;


            InputDevice currentInputDevice = GetCurrentInputDevice();

            if ((DeviceIsKeyboardOrMouse(currentInputDevice) && DeviceIsKeyboardOrMouse(device))
                || (currentInputDevice == device))
            {
                //current device is still being used. Cancel a requested device change if there is any
                CancelRequestedDeviceDisplayChange();
                inputDeviceToChangeTo = null;
                return;
            }

            if(inputDeviceToChangeTo == device
            || (DeviceIsKeyboardOrMouse(inputDeviceToChangeTo) && DeviceIsKeyboardOrMouse(device)))
            {
                //already changing to this device
                return;
            }

            //InputIconsLogger.Log("handle device change: "+device.name);
            RequestDeviceDisplayChange(device); //change displayed icons after a delay
        }

        public static void RequestDeviceDisplayChange(InputDevice device)
        {
            if (deviceDisplayChanger == null)
            {
                GameObject obj = new GameObject("II_DeviceDisplayChanger");
                deviceDisplayChanger = obj.AddComponent<II_ShowDeviceIconsAfterDelay>();
            }
            CancelRequestedDeviceDisplayChange();
            inputDeviceToChangeTo = device;

            deviceDisplayChanger.ScheduleDeviceDisplayChange(device, Instance.deviceChangeDelay);
        }

        public static void CancelRequestedDeviceDisplayChange()
        {
            if (deviceDisplayChanger == null)
            {
                return;
            }
            deviceDisplayChanger.CancelScheduledDeviceChange();
        }

        public static void SetDeviceAndRefreshDisplayedIcons(InputDevice device)
        {
            if (device == null)
            {
                ShowDeviceIconsBasedOnSettings();
                return;
            }
      

            SetCurrentInputDevice(device);
            InputDevice deviceToDisplay = GetCurrentInputDevice();

            if (device is Gamepad &&
                Instance.gamepadIconDisplaySetting == GamepadIconDisplaySetting.FirstConnected && Instance.firstConnectedGamepad != null)
            {
                //Debug.Log("gamepad is first connected gamepad: "+Instance.firstConnectedGamepad);
                deviceToDisplay = Instance.firstConnectedGamepad;
            }

            InputIconSetConfiguratorSO.UpdateCurrentIconSet(deviceToDisplay);

            //InputIconsLogger.Log("device to display: " + deviceToDisplay.name);
            
            Instance.UpdateInputStyleData();
            onControlsChanged?.Invoke(deviceToDisplay);
            inputDeviceToChangeTo = deviceToDisplay;

            InputIconsUtility.RefreshAllTMProUGUIObjects();
        }


        public void CreateInputStyleData()
        {
            if (InputIconSetConfiguratorSO.Instance == null)
            {
                InputIconsLogger.LogWarning("InputIconSetConfigurator Instance was null, please try again.");
                return;
            }

            //create data for keyboard style list
            CreateKeyboardInputStyleData(InputIconSetConfiguratorSO.Instance.keyboardIconSet.iconSetName);
            inputStyleKeyboardDataList = GetCleanedUpStyleList(inputStyleKeyboardDataList);


            //create data for gamepad style list
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();
            InputIconSetBasicSO gamepadIconSet = lastGamepadIconSet;

            if(iconSet!=null)
            {
                if (iconSet.GetType() == typeof(InputIconSetGamepadSO))
                    gamepadIconSet = iconSet;
            }

            if (gamepadIconSet == null)
                gamepadIconSet = InputIconSetConfiguratorSO.Instance.fallbackGamepadIconSet;

            CreateGamepadInputStyleData(gamepadIconSet.iconSetName);
            inputStyleGamepadDataList = GetCleanedUpStyleList(inputStyleGamepadDataList);

            UpdateTMProStyleSheetWithUsedPlayerInputs();
        }

        public void UpdateInputStyleData()
        {
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();
            if (iconSet.GetType() == typeof(InputIconSetKeyboardSO))
            {
                if(Keyboard.current==null)
                {
                    CreateKeyboardInputStyleData(iconSet.iconSetName);
                    inputStyleKeyboardDataList = GetCleanedUpStyleList(inputStyleKeyboardDataList);
                }
                else if(Keyboard.current.keyboardLayout != lastKeyboardLayout)
                {
                    CreateKeyboardInputStyleData(iconSet.iconSetName);
                    inputStyleKeyboardDataList = GetCleanedUpStyleList(inputStyleKeyboardDataList);
                }
            }

            if (iconSet.GetType() == typeof(InputIconSetGamepadSO))
            {
                if(iconSet != lastGamepadIconSet)
                {
                    CreateGamepadInputStyleData(iconSet.iconSetName);
                    inputStyleGamepadDataList = GetCleanedUpStyleList(inputStyleGamepadDataList);
                }
            }

            UpdateTMProStyleSheetWithUsedPlayerInputs();
        }

        public void CreateKeyboardInputStyleData(string deviceDisplayName)
        {
            if(Keyboard.current!=null)
                lastKeyboardLayout = Keyboard.current.keyboardLayout;

            inputStyleKeyboardDataList = InputIconsUtility.CreateInputStyleData(usedActionAssets, controlSchemeName_Keyboard, deviceDisplayName);
        }

        public void CreateGamepadInputStyleData(string deviceDisplayName)
        {
            lastGamepadIconSet = InputIconSetConfiguratorSO.GetIconSet(deviceDisplayName);
           
            inputStyleGamepadDataList = InputIconsUtility.CreateInputStyleData(usedActionAssets, controlSchemeName_Gamepad, deviceDisplayName);
        }

        public InputStyleData GetCleanedUpStyle(InputStyleData style, int index, List<InputStyleData> styleList)
        {
            //setup the single style tag fields
            style.inputStyleString_singleInput = style.inputStyleString;
            style.humanReadableString = style.humanReadableString.ToUpper();
            style.humanReadableString_singleInput = style.humanReadableString;
            style.fontCode_singleInput = style.fontCode;


            List<string> combinedBindingNames = new List<string>();
            bool combinedABinding;

            combinedABinding = false;
            for (int j = 0; j < styleList.Count; j++)
            {
                if (styleList[j] == style)
                    continue;

                if (style.bindingName == styleList[j].bindingName
                    && (style.isComposite || style.isPartOfComposite || styleList[j].isComposite || styleList[j].isPartOfComposite))
                {
                    //combine composites and part of composites
                    style.inputStyleString += multipleInputsDelimiter + styleList[j].inputStyleString;
                    style.humanReadableString += multipleInputsDelimiter + styleList[j].humanReadableString;
                    style.fontCode += multipleInputsDelimiter + styleList[j].fontCode;


                    styleList.RemoveAt(j);
                    j--;

                    continue;
                }


                if (!style.isComposite && !style.isPartOfComposite)
                {
                    if (styleList[j].bindingName == style.bindingName
                        && !combinedBindingNames.Contains(style.bindingName))
                    {
                        //combine single button bindings (e.g. if there are multiple bindings to a jump action)
                        style.inputStyleString += multipleInputsDelimiter + styleList[j].inputStyleString;
                        style.humanReadableString += multipleInputsDelimiter + styleList[j].humanReadableString;
                        style.fontCode += multipleInputsDelimiter + styleList[j].fontCode;

                        combinedABinding = true;
                    }
                }
            }


            if (combinedABinding)
                combinedBindingNames.Add(style.bindingName);

            int c = 2;
            for (int j = 0; j < styleList.Count; j++)
            {
                if (style.bindingName == styleList[j].bindingName) //make multiple binding names distinct by adding a counter at the end
                {
                    if (index < j)
                    {
                        styleList[j].bindingName += "/" + c;
                        c++;
                    }
                }

                styleList[j].tmproReferenceText = "<style=" + styleList[j].bindingName + ">";
                styleList[j].fontReferenceText = "<style=font/" + styleList[j].bindingName + ">";
            }

            return style;
        }

        /// <summary>
        /// Removes bindings which are only available in one of the style lists. E.g. if Jump/3 is only available for keyboard, remove it,
        /// since it could not be displayed when using a gamepad
        /// </summary>
        private List<InputStyleData> GetCleanedUpStyleList(List<InputStyleData> styleList)
        {

            for (int i = styleList.Count-1; i >= 0; i--) //remove empty entries
            {
                if (styleList[i].bindingName == null)
                {
                    styleList.RemoveAt(i);
                }
            }

            for(int i=0; i< styleList.Count; i++)
            {
                styleList[i] = GetCleanedUpStyle(styleList[i], i, styleList);
            }

            return styleList;
        }

        public string GetCustomStyleTag(InputStyleData styleData)
        {
            if (showAllInputOptionsInStyles)
            {
                switch (displayType)
                {
                    case DisplayType.Sprites:
                        return styleData.inputStyleString;

                    case DisplayType.Text:
                        return styleData.humanReadableString;

                    case DisplayType.TextInBrackets:
                        return "[" + styleData.humanReadableString+"]";

                    default:
                        break;
                }
            }
            else
            {
                switch (displayType)
                {
                    case DisplayType.Sprites:
                        return styleData.inputStyleString_singleInput;

                    case DisplayType.Text:
                        return styleData.humanReadableString_singleInput;

                    case DisplayType.TextInBrackets:
                        return "[" + styleData.humanReadableString_singleInput + "]";

                    default:
                        break;
                }
            }

            return "";
        }

        public string GetCustomStyleTag(InputAction action, InputBinding binding)
        {
            if(showAllInputOptionsInStyles)
            {
                switch (displayType)
                {
                    case DisplayType.Sprites:
                        return GetSpriteStyleTag(action, binding);

                    case DisplayType.Text:
                        return GetHumanReadableString(action, binding);

                    case DisplayType.TextInBrackets:
                        return "[" + GetHumanReadableString(action, binding) + "]";

                    default:
                        break;
                }
            }
            else
            {
                switch (displayType)
                {
                    case DisplayType.Sprites:
                        return GetSpriteStyleTagSingle(action, binding);

                    case DisplayType.Text:
                        return GetHumanReadableStringSingle(action, binding);

                    case DisplayType.TextInBrackets:
                        return "[" + GetHumanReadableStringSingle(action, binding) + "]";

                    default:
                        break;
                }
            }
            
            return "";
        }

        public InputStyleData GetInputStyleData(string bindingName)
        {
            List<InputStyleData> styleList = inputStyleKeyboardDataList;
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();

            if (iconSet.GetType() == typeof(InputIconSetGamepadSO))
            {
                styleList = inputStyleGamepadDataList;
            }

            for (int i = 0; i < styleList.Count; i++)
            {
                if (styleList[i].bindingName == bindingName)
                    return styleList[i];
            }

            return null;
        }

        public InputStyleData GetInputStyleDataSpecific(string bindingName, bool gamepad)
        {
            List<InputStyleData> styleList;

            if (gamepad)
            {
                styleList = inputStyleGamepadDataList;
            }
            else
            {
                styleList = inputStyleKeyboardDataList;
            }

            for (int i = 0; i < styleList.Count; i++)
            {
                if (styleList[i].bindingName == bindingName)
                    return styleList[i];
            }

            return null;
        }

        public string GetBindingName(InputAction action, InputBinding binding)
        {
            string bindingName = action.actionMap.name+"/"+action.name;

            if (!binding.isComposite)
            {
                bindingName += "/" + binding.name;
            }

            return bindingName;
        }

        public string GetSpriteStyleTag(InputAction action, InputBinding binding)
        {
            string bindingName = GetBindingName(action, binding);
            InputStyleData data = GetInputStyleData(bindingName);
            return data != null ? data.inputStyleString : "";
        }

        public string GetSpriteStyleTagSingle(string bindingName)
        {
            InputStyleData data = GetInputStyleData(bindingName);
            return data != null ? data.inputStyleString_singleInput : "";
        }

        public string GetSpriteStyleTagSingle(InputAction action, InputBinding binding)
        {
            string bindingName = GetBindingName(action, binding);
            InputStyleData data = GetInputStyleData(bindingName);
            return data != null ? data.inputStyleString_singleInput : "";
        }

        public string GetHumanReadableString(InputAction action, InputBinding binding)
        {
            string bindingName = GetBindingName(action, binding);
            InputStyleData data = GetInputStyleData(bindingName);
            return data != null ? data.humanReadableString : "";
        }

        public string GetHumanReadableStringSingle(InputAction action, InputBinding binding)
        {
            string bindingName = GetBindingName(action, binding);
            InputStyleData data = GetInputStyleData(bindingName);
            return data != null ? data.humanReadableString_singleInput : "";
        }


        public static string GetActionStringRenaming(string name)
        {
            
            for(int i=0; i<Instance.actionNameRenamings.Count; i++)
            {
                if (Instance.actionNameRenamings[i].originalString.ToUpper() == name.ToUpper())
                    return Instance.actionNameRenamings[i].outputString.ToUpper();
            }
            return name;
        }


        public List<string> GetAllBindingNames()
        {
            List<string> output = new List<string>();
            for(int i=0; i<inputStyleKeyboardDataList.Count; i++)
            {
                output.Add(inputStyleKeyboardDataList[i].bindingName);
                output.Add("Font/" + inputStyleKeyboardDataList[i].bindingName);
            }

            for (int i = 0; i < inputStyleGamepadDataList.Count; i++)
            {
                if(!output.Contains(inputStyleGamepadDataList[i].bindingName))
                {
                    output.Add(inputStyleGamepadDataList[i].bindingName);
                    output.Add("Font/" + inputStyleGamepadDataList[i].bindingName);
                }
               
            }
            return output;
        }

      
        public static InputDevice GetCurrentInputDevice()
        {
            return Instance.currentInputDevice;
        }

        public static void SetCurrentInputDevice(InputDevice device)
        {
            if (device == null)
            {
                Debug.Log("device is null");
                return;
            }
            
            Instance.currentInputDevice = device;
            inputDeviceToChangeTo = device;
        }


        public static void UpdateTMProStyleSheetWithUsedPlayerInputs()
        {

            InputIconSetBasicSO iconSetSO = InputIconSetConfiguratorSO.GetCurrentIconSet();
            if (iconSetSO == null)
                return;


            if (iconSetSO.controlSchemeName == InputIconsManagerSO.Instance.controlSchemeName_Keyboard)
                InputIconsUtility.OverrideStylesInStyleSheetDeviceSpecific(false);
            else
                InputIconsUtility.OverrideStylesInStyleSheetDeviceSpecific(true);

            onStyleSheetsUpdated?.Invoke();

        }

        public static void UpdateStyleData()
        {
            Instance.CreateInputStyleData();
        }

        public static void RegisterInputIconsText(InputIconsText inputIconsText)
        {
            activeTexts.Add(inputIconsText);
            inputIconsText.SetDirty();
        }

        public static void UnregisterInputIconsText(InputIconsText inputIconsText)
        {
            activeTexts.Remove(inputIconsText);
        }

        public static void RefreshInputIconsTexts()
        {
            foreach(InputIconsText iconsText in activeTexts)
            {
                iconsText.SetDirty();
            }
        }

        public static void ResetAllBindings()
        {
            foreach (InputActionAsset asset in Instance.usedActionAssets)
            {
                asset.RemoveAllBindingOverrides();
            }

            SaveUserBindings();
            HandleInputBindingsChanged();
            onBindingsReset?.Invoke();
        }

        public static void SaveUserBindings()
        {
            if (!Instance.loadAndSaveInputBindingOverrides)
                return;

            OverrideBindingDataWrapperClass bindingList = new OverrideBindingDataWrapperClass();

            foreach(InputActionAsset asset in Instance.usedActionAssets)
            {
                foreach(InputActionMap map in asset.actionMaps)
                {
                    foreach (InputBinding binding in map.bindings)
                    {
                        if (binding.overridePath != null)
                        {
                            bindingList.bindingList.Add(new OverrideBindingData(binding.id.ToString(), binding.overridePath));
                        }
                    }
                }
            }

            PlayerPrefs.SetString("II-Rebinds_", JsonUtility.ToJson(bindingList));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the override path to "" for every binding which has the given effective override path, with the exception of the "bindingException".
        /// Can be used to correctly remove bindings once they got overriden in a previous session for example
        /// </summary>
        /// <param name="map"></param>
        /// <param name="overridePath"></param>
        /// <param name="bindingException"></param>
        public static void SetOverridesEmpty(InputActionMap map, string overridePath, InputBinding bindingException)
        {
            var bindings = map.bindings;
            for (int i = 0; i < bindings.Count; ++i)
            {
                if (bindings[i].id != bindingException.id && bindings[i].effectivePath == overridePath)
                {
                    //if there is an override apply it
                    //InputIconsLogger.Log("setting override path empty: " + i);
                    map.ApplyBindingOverride(i, new InputBinding { overridePath = "" });
                }
            }
        }

        public static void LoadUserRebinds()
        {
            if (!Instance.loadAndSaveInputBindingOverrides)
            {
                Instance.CreateInputStyleData();
                return;
            }
           
            if (PlayerPrefs.HasKey("II-Rebinds_"))
            {
                Dictionary<System.Guid, string> overrides = GetSavedBindingOverrides(); 

                //walk through action maps check dictionary for overrides
                foreach (InputActionAsset asset in Instance.usedActionAssets)
                {
                    foreach (InputActionMap map in asset.actionMaps)
                    {
                        var bindings = map.bindings;
                        for (int i = 0; i < bindings.Count; ++i)
                        {
                            if (overrides.TryGetValue(bindings[i].id, out string overridePath))
                            {
                                //if there is an override apply it
                                //InputIconsLogger.Log("override binding found: " + overridePath);
                                map.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
                                
                                //find other bindings which might have the same binding and remove it from them
                                SetOverridesEmpty(map, overridePath, bindings[i]);
                            }
                        }
                    }
                }
            }

            Instance.CreateInputStyleData();
        }

        public static Dictionary<System.Guid, string> GetSavedBindingOverrides()
        {
            OverrideBindingDataWrapperClass bindingList = JsonUtility.FromJson ( PlayerPrefs.GetString ( "II-Rebinds_" ), typeof (OverrideBindingDataWrapperClass) ) as OverrideBindingDataWrapperClass;

            if (bindingList == null)
                return new Dictionary<Guid, string>();

            //create a dictionary to easier check for existing overrides
            Dictionary<System.Guid, string> overrides = new Dictionary<System.Guid, string> ();
            foreach (OverrideBindingData item in bindingList.bindingList)
            {
                overrides.Add(new System.Guid(item.id), item.path);
            }

            return overrides;
        }

        /// <summary>
        /// Can be used to apply binding overrides to the used Input Action Assets.
        /// Use this if you don't use the Rebind Prefabs that come with this asset,
        /// or if you apply binding overrides somewhere else and they should be reflected by Input Icons.
        /// </summary>
        public static void ApplyBindingOverridesToInputActionAssets(InputAction action, int bindingIndex, string overridePath)
        {
            bool bindingChanged = false;
            foreach (InputActionAsset inputActionAsset in Instance.usedActionAssets)
            {
                foreach (InputActionMap actionMap in inputActionAsset.actionMaps)
                {
                    InputAction otherAction = actionMap.FindAction(action.id);
                    if (otherAction != null)
                    {
                        InputBinding otherBinding = otherAction.bindings[bindingIndex];
                        if (otherBinding.overridePath != null && overridePath == null)
                        {
                            //Override Path was removed
                            otherAction.RemoveBindingOverride(bindingIndex);
                            bindingChanged = true;
                        }
                        else if (otherBinding.overridePath == null && overridePath != null)
                        {
                            //Path was overriden
                            otherAction.ApplyBindingOverride(bindingIndex, overridePath);
                            bindingChanged = true;
                        }
                        else if (otherBinding.overridePath != overridePath)
                        {
                            //Path was changed
                            otherAction.ApplyBindingOverride(bindingIndex, overridePath);
                            bindingChanged = true;
                        }

                        break;
                    }
                }
            }

            if (bindingChanged)
            {
                HandleInputBindingsChanged();
            }
        }


        //Removes all InputIcons related entries from the default style sheet. Can only remove entries which are still present
        //in the list of Input Action Assets assigned to the manager
        public static void RemoveAllStyleSheetEntries()
        {
            TMP_InputStyleHack.RemoveAllEntries();
        }

        //Used during setup. Adds empty values to the style sheet which can later be filled with actual values
        public static int PrepareAddingInputStyles(List<InputStyleData> inputStyles)
        {
            List<TMP_InputStyleHack.StyleStruct> styleUpdates = new List<TMP_InputStyleHack.StyleStruct>();
            int c = 0;
            for (int i = 0; i < inputStyles.Count*2; i++)
            {
                styleUpdates.Add(new TMP_InputStyleHack.StyleStruct("", "", ""));
                c++;
            }

            TMP_InputStyleHack.PrepareCreateStyles(styleUpdates);
            onStylesPrepared?.Invoke();
            return c;
        }

        //Used during setup to add the necessary entries to the style sheet
        public static int AddInputStyles(List<InputStyleData> inputStyles)
        {

            List<TMP_InputStyleHack.StyleStruct> styleUpdates = new List<TMP_InputStyleHack.StyleStruct>();

            for (int i = 0; i < inputStyles.Count; i++)
            {
                styleUpdates.Add(new TMP_InputStyleHack.StyleStruct(inputStyles[i].bindingName, inputStyles[i].inputStyleString, ""));
            }

            if(Instance.isUsingFonts)
            {
                for (int i = 0; i < inputStyles.Count; i++)
                {
                    styleUpdates.Add(new TMP_InputStyleHack.StyleStruct("Font/" + inputStyles[i].bindingName, inputStyles[i].inputStyleString, ""));
                }
            }
           

            int styleCount = TMP_InputStyleHack.CreateStyles(styleUpdates);
            onStylesAddedToStyleSheet?.Invoke();
            return styleCount;
        }


        /// <summary>
        /// Private wrapper class for json serialization of the overrides
        /// </summary>
        [System.Serializable]
        class OverrideBindingDataWrapperClass
        {
            public List<OverrideBindingData> bindingList = new List<OverrideBindingData> ();
        }

        [Serializable]
        private class OverrideBindingData
        {
            public string id;
            public string path;

            public OverrideBindingData(string bindingId, string bindingPath)
            {
                id = bindingId;
                path = bindingPath;
            }
        }

    }
}
