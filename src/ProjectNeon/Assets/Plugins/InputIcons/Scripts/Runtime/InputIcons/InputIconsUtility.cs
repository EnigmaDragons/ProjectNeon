using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace InputIcons
{
    public class InputIconsUtility : MonoBehaviour
    {

        [System.Serializable]
        public class InputStyleData
        {
            public bool isComposite = false;
            public bool isPartOfComposite = false;
            public string tmproReferenceText;  //<style=Controls/Action/compositePart>
            public string bindingName; //Controls/Action/compositePart
            public string inputStyleString;  //<sprite=....><sprite=...><sprite=...>...
            public string inputStyleString_singleInput;  //<sprite=....>
            public string humanReadableString; //WASD or Arrows
            public string humanReadableString_singleInput; //WASD
            public string fontReferenceText; ////<style=Font/Controls/Action/compositePart>
            public string fontCode; //E905, E95B, ...
            public string fontCode_singleInput;

            public InputStyleData()
            {
            }

            public InputStyleData(string bName, bool isComp, bool isPartOfComp, string tmproText, string style, string humanReadableStr, string fontC, string fontReference)
            {
                tmproReferenceText = tmproText;
                bindingName = bName; 
                inputStyleString = style;
                humanReadableString = humanReadableStr;
                isComposite = isComp;
                isPartOfComposite = isPartOfComp;
                fontReferenceText = fontReference;
                fontCode = fontC;
            }
        }

        public enum BindingType { Up, Down, Left, Right, Forward, Backward };

        public enum DeviceType { Auto, KeyboardAndMouse, Gamepad };

        public static string GetStyleName(InputAction action)
        {
            return action.actionMap.name + "/" + action.name;
        }

        public static string GetSpriteName(InputBinding binding, string controlSchemeName, bool forOpeningTag)
        {
            string keyName = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            if (controlSchemeName == InputIconsManagerSO.Instance.controlSchemeName_Keyboard)
            {
                keyName = GetKeyboardString(binding, forOpeningTag);
            }
            return keyName;
        }

        public static string GetSpriteNameOfActionForSpriteDisplay(InputAction action, BindingType bindingType, string controlSchemeName)
        {
            int index = GetIndexOfBindingType(action, bindingType, controlSchemeName);
            if (index >= 0)
            {
                string keyName = GetSpriteName(action.bindings[index], controlSchemeName, true);
                return keyName;
            }
            return "";
        }

        public static string GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(InputAction action, BindingType bindingType, bool gamepad)
        {
            string controlSchemeName = InputIconsManagerSO.Instance.controlSchemeName_Keyboard;
            if (gamepad)
                controlSchemeName = InputIconsManagerSO.Instance.controlSchemeName_Gamepad;

            int index = GetIndexOfBindingType(action, bindingType, controlSchemeName);
            if (index >= 0)
            {
                string keyName = GetSpriteName(action.bindings[index], controlSchemeName, true);
                return keyName;
            }
            return "";
        }

        public static string GetSpriteNameOfActionForSpriteDisplayDeviceSpecific(InputAction action, int bindingIndex, bool gamepad)
        {
            string controlSchemeName = InputIconsManagerSO.Instance.controlSchemeName_Keyboard;
            if (gamepad)
                controlSchemeName = InputIconsManagerSO.Instance.controlSchemeName_Gamepad;

            string keyName = GetSpriteName(action.bindings[bindingIndex], controlSchemeName, true);
            return keyName;
        }

        /*public static string GetSpriteNameOfActionForSpriteDisplay(InputAction action, int bindingIndex, string controlSchemeName)
        {
            if (bindingIndex >= 0)
            {
                string keyName = GetSpriteName(action.bindings[bindingIndex], controlSchemeName, true);
                return keyName;
            }
            return "";
        }

        public static string GetSpriteNameDeviceSpecific(InputBinding binding, bool forOpeningTag, bool gamepad)
        {
            string keyName = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            if (!gamepad)
            {
                keyName = GetKeyboardString(binding, forOpeningTag);
            }
            return keyName;
        }*/

        public static List<string> GetKeyNamesOfAction(InputAction action, string controlSchemeName)
        {
            List<string> keyNames = new List<string>();
            for(int i=0; i<action.bindings.Count; i++)
            {
                if (action.bindings[i].groups.Contains(controlSchemeName))
                {
                    if (!action.bindings[i].isPartOfComposite)
                    {
                        string keyName = GetSpriteName(action.bindings[i], controlSchemeName, false);

                        keyNames.Add(keyName);
                    }
                    else
                    {
                        for (int j = i; j < action.bindings.Count; j++)
                        {
                            if (!action.bindings[j].isPartOfComposite)
                                break;

                            string keyName = GetSpriteName(action.bindings[j], controlSchemeName, false);
                            keyNames.Add(keyName);
                        }
                    }
                    return keyNames;
                }
            }
            return keyNames;
        }

        public static InputStyleData GetStyleOpeningTagOfComposite(InputAction action, InputBinding binding, string deviceDisplayName, string controlSchemeName)
        {
            if(!binding.isComposite)
            {
                InputIconsLogger.LogError("composite binding expected, but non composite found");
                return null;
            }

            InputStyleData styleData = new InputStyleData();
            string compositeDelimiter = InputIconsManagerSO.Instance.compositeInputDelimiter;

            string bindingName = action.actionMap.name + "/" + action.name;
            styleData.fontReferenceText = "<style=font/" + bindingName + ">";

            bool isCorrectComposite = false;
            for(int i=0; i<action.bindings.Count; i++)
            {
                if(action.bindings[i].isComposite)
                {
                    //Debug.Log(action.bindings[i].name + " " + binding.name);
                    isCorrectComposite = action.bindings[i].name == binding.name;
                }

                if(!action.bindings[i].isComposite && isCorrectComposite && action.bindings[i].isPartOfComposite)
                {
                    //s += GetStyleOpeningTag(action, action.bindings[i], deviceDisplayName, controlSchemeName);

                    InputStyleData sPart = GetStyleOpeningTag(action, action.bindings[i], deviceDisplayName, controlSchemeName);
                    //                    InputStyleData sPart = new InputStyleData();

                    if (sPart == null)
                        continue;

                    if(styleData.bindingName == null)
                    {
                        string[] stringParts = sPart.bindingName.Split('/');
                        for(int n=0; n<stringParts.Length-1; n++)
                        {
                            styleData.bindingName += stringParts[n];
                            if (n < stringParts.Length - 2)
                                styleData.bindingName += "/";
                        }
                        
                    }

                    styleData.isComposite = true;
                    styleData.humanReadableString += sPart.humanReadableString + compositeDelimiter;
                    styleData.inputStyleString += sPart.inputStyleString;
                    styleData.tmproReferenceText += sPart.tmproReferenceText + " ";
                    styleData.fontCode += sPart.fontCode;
                }

            
            }

            if (styleData.humanReadableString != null)
            {
                //Debug.Log(styleData.humanReadableString.ElementAt(styleData.humanReadableString.Length - 2));
                if (styleData.humanReadableString.ElementAt(styleData.humanReadableString.Length - 2) == ',')
                {
                    styleData.humanReadableString = styleData.humanReadableString.Remove(styleData.humanReadableString.Length - 2);
                }
                   
            }
             

            return styleData;
        }

        /// <summary>
        /// Creates and returns a Style Opening Tag for the default style sheet for a specific binding.
        /// </summary>
        public static InputStyleData GetStyleOpeningTag(InputAction action, InputBinding binding, string deviceDisplayName, string controlSchemeName)
        {
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetIconSet(deviceDisplayName);
            if(iconSet==null)
            {
                InputIconsLogger.LogWarning("Could not find icon set with device display name: " + deviceDisplayName + ". Check your Input Icon Sets in " +
                    "Assets/Input Icons");

                if (controlSchemeName == InputIconsManagerSO.Instance.controlSchemeName_Keyboard)
                    iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                else
                    iconSet = InputIconSetConfiguratorSO.Instance.xBoxIconSet;
            }

            if (binding.isComposite)
            {
                return GetStyleOpeningTagOfComposite(action, binding, deviceDisplayName, controlSchemeName);
            }


            if (binding.groups.Length == 0)
            {
                InputIconsLogger.LogWarning("There is no Control Scheme set for the Action " + action.name + " in the Input Action Asset: " + action.actionMap+"\n" +
                    "Please make sure to set up control schemes and to assign the proper devices to these control schemes.");
            }

            if (!binding.groups.Contains(controlSchemeName))
            {
                return null;
            }

            string bindingTag = GetSpriteName(binding, controlSchemeName, true);

            //Handle the three types of Shift, Alt and Ctrl. There is "shift", "leftShift" and "rightShift"
            //just shift gets converted to left shift, the same for alt and ctrl
            if (bindingTag == "shift") bindingTag = "leftShift";
            if (bindingTag == "alt") bindingTag = "leftAlt";
            if (bindingTag == "ctrl") bindingTag = "leftCtrl";


            if (bindingTag == InputIconsManagerSO.Instance.textDisplayForUnboundActions)//if there is no binding, return empty string to display the unbound sprite
            {
                bindingTag = "";
            }
            else if (InputIconsManagerSO.Instance.displayType == InputIconsManagerSO.DisplayType.Sprites &&
                !iconSet.HasSprite(bindingTag))//if sprite of binding tag is not available, use custom fallback sprite instead of default TMP fallback
            {
                bindingTag = "FallbackSprite";
            }


            string styleOpeningTag = "<sprite=\"" + deviceDisplayName + "\" name=\"" + bindingTag.ToUpper() + "\"";
            if (InputIconsManagerSO.Instance.tintingEnabled)
                styleOpeningTag += ", tint=1";
            
             styleOpeningTag += ">";

            string bindingName = action.actionMap.name + "/" + action.name;
            if (binding.name != "")
                bindingName += "/" + binding.name;

           
            string tmproReference = "<style="+bindingName+">";
            string fontReference = "<style=font/"+bindingName+">";


            string humanReadableString = GetHumanReadableActionName(action, binding, controlSchemeName);
            //Debug.Log(humanReadableString);
            humanReadableString = InputIconsManagerSO.GetActionStringRenaming(humanReadableString);

            //humanReadableString = humanReadableString.Replace("{", "");
            //humanReadableString = humanReadableString.Replace("}", "");

            if (humanReadableString == "")
            {
                humanReadableString = InputIconsManagerSO.Instance.textDisplayForUnboundActions;
            }

            //Debug.Log(tmproReference + ": " + bindingTag);
            string fontCode = "\\u" + GetFontCodeOfDevice(bindingTag, deviceDisplayName);
            //Debug.Log(humanReadableString);

            //if (InputIconsManagerSO.Instance.displayType == InputIconsManagerSO.DisplayType.TextInBrackets)
            //styleOpeningTag = "[" + styleOpeningTag + "]";


            InputStyleData isd = new InputStyleData(bindingName, binding.isComposite, binding.isPartOfComposite, tmproReference, styleOpeningTag, humanReadableString, fontCode, fontReference);

            return isd;
            

        }

        public static string GetStyleOpeningTag(InputAction action, string deviceDisplayName, string controlSchemeName)
        {
            InputIconSetBasicSO iconSet;
            if (controlSchemeName == InputIconsManagerSO.Instance.controlSchemeName_Keyboard)
                iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
            else
                iconSet = InputIconSetConfiguratorSO.Instance.xBoxIconSet;

            if (iconSet == null)
            {
                InputIconsLogger.LogWarning("Could not find icon set with device display name: " + deviceDisplayName + ". Check your Input Icon Sets in " +
                    "Assets/Input Icons");
                return "";
            }

            List<List<string>> allStyleParts = new List<List<string>>();
            List<string> currentStyleParts = new List<string>();

            for (int j = 0; j < action.bindings.Count; j++)
            {

                bool isComposite = action.bindings[j].isComposite;

                if (!action.bindings[j].groups.Contains(controlSchemeName))
                {
                    continue;
                }

                string bindingTag = GetSpriteName(action.bindings[j], controlSchemeName, true);

                if (bindingTag == InputIconsManagerSO.Instance.textDisplayForUnboundActions)//if there is no binding, return empty string to display the unbound sprite
                {
                    bindingTag = "";
                }
                else if (!iconSet.HasSprite(bindingTag))//if sprite of binding tag is not available, use custom fallback sprite instead of default TMP fallback
                {
                    bindingTag = "FallbackSprite";
                }



                bool nextIsAnotherBinding = false;
                if (j < action.bindings.Count - 1)
                {
                    if (action.bindings[j + 1].isComposite || !action.bindings[j+1].isPartOfComposite)
                        nextIsAnotherBinding = true;
                }
                else
                    nextIsAnotherBinding = true;
             
                
                if(!isComposite)
                {
                    currentStyleParts.Add("<sprite=\"" + deviceDisplayName + "\" name=\"" + bindingTag.ToUpper() + "\">");
                }

                if (nextIsAnotherBinding)
                {

                    string debugString = "";
                    foreach (string s in currentStyleParts)
                        debugString += s;


                    allStyleParts.Add(currentStyleParts);

                    if (!InputIconsManagerSO.Instance.showAllInputOptionsInStyles)
                        break;

                    List<string> delimiterStyle = new List<string>();
                    delimiterStyle.Add(InputIconsManagerSO.Instance.multipleInputsDelimiter);
                    allStyleParts.Add(delimiterStyle);

                    currentStyleParts = new List<string>();
             
                }
            }


            string output = "";
            List<string> outputParts = GetCleanedUpStyles(allStyleParts);
            

            for (int i = 0; i < outputParts.Count; i++)
            {
                output += outputParts[i];
            }

            return output;
        }

        public static string GetHumanReadableActionNameOfComposite(InputAction action, InputBinding binding, string controlSchemeName)
        {
            string s = "";
            bool found = false;
            for (int i = 0; i < action.bindings.Count; i++)
            {

                if (found)
                    break;

                if (action.bindings[i] == binding && binding.isComposite)
                {
                    found = true;
                    bool lastWasComposite = true;
                    for (int j = i; j < action.bindings.Count; j++)
                    {
                        if (action.bindings[j].isComposite)//&& action.bindings[j+1].groups.Contains(controlSchemeName))
                        {
                            lastWasComposite = true;
                            continue;
                        }

                        if (action.bindings[j].groups.Contains(controlSchemeName))
                        {
                            if (s.Length > 0)
                            {
                                if (lastWasComposite)
                                    s += InputIconsManagerSO.Instance.multipleInputsDelimiter;
                                else
                                    s += ", ";
                            }


                            s += GetHumanReadableActionName(action, action.bindings[j], controlSchemeName);
                        }

                        lastWasComposite = false;

                    }
                }
            }

            return s;
        }
        
        public static string GetHumanReadableActionName(InputAction action, InputBinding binding, string controlSchemeName)
        {

            if (binding.isComposite)
            {
                return GetHumanReadableActionNameOfComposite(action, binding, controlSchemeName);
            }

            string s = "";

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i] == binding)
                {
                    if (binding.groups.Length == 0)
                    {
                        InputIconsLogger.LogWarning("There is no Control Scheme set for the Action " + action.name + " in the Input Action Asset: " + action.actionMap + "\n" +
                            "Please make sure to set up control schemes and to assign the proper devices to these control schemes.");
                    }

                    if (!binding.groups.Contains(controlSchemeName))
                    {
                        continue;
                    }

                    string bindingTag = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

                    if (controlSchemeName == InputIconsManagerSO.Instance.controlSchemeName_Keyboard)
                    {
                        bindingTag = GetKeyboardString(binding, false);
                    }


                    if (s.Length > 0)
                    {
                        if (InputIconsManagerSO.Instance.showAllInputOptionsInStyles)
                        {
                            s += InputIconsManagerSO.Instance.multipleInputsDelimiter;
                        }
                    }

                    s += bindingTag.ToUpper();

                    if (!InputIconsManagerSO.Instance.showAllInputOptionsInStyles)
                        return s;

                }
            }

            return s.Trim();
        }

        
        /// <summary>
        /// Cleans up the styles and removes any unnecessary characters
        /// </summary>
        /// <param name="styles"></param>
        /// <returns></returns>
        private static List<string> GetCleanedUpStyles(List<List<string>> styles)
        {
            List<string> outputParts = new List<string>();
            int unboundCount = 0;
            int delimiterCount = 0;
            string delimiter = InputIconsManagerSO.Instance.multipleInputsDelimiter;

            for (int i = 0; i < styles.Count; i++) //combine composites together
            {
                string s = "";
                for (int j = 0; j < styles[i].Count; j++)
                {
                    s += styles[i][j];
                }
                //Debug.Log("add: " + s);
                outputParts.Add(s);

                if (outputParts[i].Contains("\"\""))
                {
                    unboundCount++;
                }

                if (outputParts[i] == delimiter)
                    delimiterCount++;
            }
           

            //for non-composites: remove fallback sprites if another binding is available (e.g. do not display "Jump: Space or FallbackSprite or U" ... or "Jump: FallbackSprite or Space or U")
            if(unboundCount > 0)
            {
                string[] stringSeparators = new string[] { "sprite" };
                int spriteCount = outputParts[0].Split(stringSeparators, System.StringSplitOptions.None).Length;

                //Debug.Log(spriteCount + " " + delimiterCount);

                if (spriteCount <= delimiterCount && outputParts.Count > 0)
                {
                    for (int i = outputParts.Count - 1; i >= 0; i--)
                    {
                        if (outputParts[i].Contains("\"\""))
                        {
                            outputParts.RemoveAt(i);
                        }
                    }
                }
            }
           


            for (int i = 0; i < outputParts.Count; i++) //remove double texts, like "E or E" ... or "WASD or WASD"
            {
                for (int j = outputParts.Count - 1; j >= 0; j--)
                {
                    if (i != j && outputParts[i] == outputParts[j] && outputParts[i] != delimiter)
                    {
                        outputParts.RemoveAt(j);
                    }
                }
            }

            bool delimitersRemoved;
            do
            {
                delimitersRemoved = false;
                if (outputParts.Count > 0) //remove unnecessary delimiters at the front and back if there are any
                {
                    if (outputParts[outputParts.Count - 1] == delimiter)
                    {
                        outputParts.RemoveAt(outputParts.Count - 1);
                        delimitersRemoved = true;
                    }

                    if (outputParts[0] == delimiter)
                    {
                        outputParts.RemoveAt(0);
                        delimitersRemoved = true;
                    }
                }
            } 
            while (delimitersRemoved);

            //remove duplicate delimiters in the center
            bool lastWasDelimiter = false;
            for(int i=outputParts.Count-1; i>=0; i--)
            {
                if (outputParts[i] == delimiter)
                {
                    if (lastWasDelimiter)
                    {
                        outputParts.RemoveAt(i);
                    }
                    lastWasDelimiter = true;
                }
                else
                    lastWasDelimiter = false;
            }


            return outputParts;
        }


        /// <summary>
        /// Returns a human readable string for a binding. Uses either system language or english only, depending on the setting in the InputIconsManager
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="forStyleOpeningTag"></param>
        /// <returns></returns>
        public static string GetKeyboardString(InputBinding binding, bool forStyleOpeningTag)
        {
            string[] supportedLayouts = new string[]{"00000409", "00000407" ,"0000040C"}; //english, german and french layouts

            string fullButtonPath = binding.effectivePath;
            string[] pathParts =  binding.effectivePath.ToString().Split('/');
            string buttonFullName =pathParts[pathParts.Length-1]; //e.g. <Keyboard>/semicolon -> semicolon
            string displayString = binding.ToDisplayString();

            if (fullButtonPath == "")
                return InputIconsManagerSO.Instance.textDisplayForUnboundActions;

            if (!forStyleOpeningTag)
            {
                InputControl control = InputSystem.FindControl(fullButtonPath);

                if (control is KeyControl keyControl)
                {
                    if(InputIconsManagerSO.Instance.textDisplayLanguage == InputIconsManagerSO.TextDisplayLanguage.SystemLanguage)
                    {
                        if (supportedLayouts.Contains(Keyboard.current.keyboardLayout))
                        {
                            return keyControl.displayName;
                        }
                    }
                }
                return displayString;
            }
              

            if (buttonFullName.Length == 1)
            {
                InputControl control = InputSystem.FindControl(fullButtonPath);
                if (control is KeyControl keyControl)
                {
                    if (IsNumber(buttonFullName[0])) //handle numbers
                    {
                        if (int.TryParse(buttonFullName, out int number))
                        {
                            buttonFullName = "Digit" + buttonFullName;
                        }
                    }
                    else if (IsLetter(buttonFullName[0]) &&
                         supportedLayouts.Contains(Keyboard.current.keyboardLayout)) //Handle different keyboard layouts (e.g. qwerty, qwertz, azerty, ...)
                    {
                        buttonFullName = keyControl.displayName;
                    }
                }

            }
            else if (fullButtonPath.Length > 0)
            {
                InputControl control = InputSystem.FindControl(fullButtonPath);
                if (control is KeyControl keyControl)
                {
                    //Handle different keyboard layouts (e.g. qwerty, qwertz, azerty, ...)
                    //try to display a letter instead of english layout special character
                    //for example french letter M is on a english special character. Try to display M 
                    if (keyControl.displayName.Length == 1)
                    {
                        if (IsLetter(keyControl.displayName[0]) &&
                         supportedLayouts.Contains(Keyboard.current.keyboardLayout))
                        {
                            buttonFullName = keyControl.displayName;
                        }
                    }
                }
            }

            buttonFullName = buttonFullName.Replace(" ", "");
            return buttonFullName;
        }

        private static bool IsNumber(char c)
        {
            int number = System.Convert.ToInt32(c);
            if (number >= 48 && number <= 57)
            {
                return true;
            }
            return false;
        }

        private static bool IsLetter(char c)
        {
            int number = System.Convert.ToInt32(c);
            if ((number >= 65 && number <= 90)
                || (number >= 97 && number <= 122))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the binding index of an action of the active device.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingType">Binding type of the action. Can be anything if the action is not a part of a composite</param>
        /// <returns></returns>
        public static int GetIndexOfBindingType(InputAction action, BindingType bindingType, string activeDeviceString)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].groups.Contains(activeDeviceString))
                {
                    if(!ActionIsComposite(action))
                    {
                        return i;
                    }
                    else if(action.bindings[i].name.ToUpper() == bindingType.ToString().ToUpper())
                    {
                      return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns all binding indexes of an action of the active device.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingType">Binding type of the action. Can be anything if the action is not a part of a composite</param>
        /// <returns></returns>
        public static List<int> GetIndexesOfBindingType(InputAction action, BindingType bindingType)
        {
            List<int> outputList = new List<int>();
            string deviceString = GetActiveDeviceString();

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].groups.Contains(deviceString))
                {
                    if (!ActionIsComposite(action))
                    {
                        outputList.Add(i);
                    }
                    else if (action.bindings[i].name.ToUpper() == bindingType.ToString().ToUpper())
                    {
                        outputList.Add(i);
                    }
                }
            }
            return outputList;
        }


        /// <returns>Returns the first binding index of the current device</returns>
        public static int GetIndexOfInputBinding(InputAction action, InputBinding binding)
        {
            string deviceString = GetActiveDeviceString();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].groups.Contains(deviceString))
                {
                    if(action.bindings[i].id == binding.id)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns all bindings of an action. Can be used after assigning an override to an action to check if
        /// there are any duplicates of bindings and to react accordingly.
        /// </summary>
        /// <returns>All bindings of an action</returns>
        public static List<InputBinding> GetBindings(InputAction action, BindingType bindingType)
        {

            List<InputBinding> foundBindings = new List<InputBinding>();

            List<int> indexes = GetIndexesOfBindingType(action, bindingType);
           
            for(int i=0; i<indexes.Count; i++)
            {
                foundBindings.Add(action.bindings[indexes[i]]);
            }
               
            return foundBindings;
        }

        public static bool ActionIsComposite(InputAction action)
        {
            string deviceString = GetActiveDeviceString();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].groups.Contains(deviceString))
                {
                    if (action.bindings[i].isPartOfComposite)
                        return true;
                }
            }

            return false;
        }

        public static string GetFontCodeOfDevice(string textMeshStyleTag, string deviceDisplayName)
        {
            InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.GetIconSet(deviceDisplayName);
            List<InputSpriteData> data = iconSet.GetAllInputSpriteData();
            for(int i=0; i<data.Count; i++)
            {
                if (data[i].textMeshStyleTag.ToUpper() == textMeshStyleTag.ToUpper())
                {
                    //Debug.Log("font code for: " + data[i].textMeshStyleTag + " is: " + data[i].fontCode);
                    return data[i].fontCode;
                }
            }

            if (textMeshStyleTag != "FallbackSprite")
                return GetFontCodeOfDevice("FallbackSprite", deviceDisplayName);

            return "";
        }

        /// <summary>
        /// Get the name of the currently used device ... Keyboard and Mouse, PS3, PS4, XBox, ...
        /// </summary>
        /// <returns></returns>
        public static string GetActiveDeviceString()
        {
            InputDevice device = InputIconsManagerSO.GetCurrentInputDevice();
            string deviceString;
            if (device is Gamepad)
            {
                deviceString = InputIconsManagerSO.Instance.controlSchemeName_Gamepad;
            }
            else
            {
                deviceString = InputIconsManagerSO.Instance.controlSchemeName_Keyboard;
            }

            return deviceString;
        }

        /// <summary>
        /// Creates updated input style data for the assigned Input Action Assets
        /// </summary>
        /// <param name="usedActionAssets">The assets for which to create style updates</param>
        /// <param name="controlSchemeName">Keyboard and Mouse or Gamepad control scheme</param>
        /// <param name="deviceString">Keyboard, PS3, PS4, XBox, ... the name of the sprite atlas</param>
        /// <returns></returns>
        public static List<InputStyleData> CreateInputStyleData(List<InputActionAsset> usedActionAssets, string controlSchemeName, string deviceString)
        {

            List <InputStyleData> outputList = new List<InputStyleData>();

            for (int k = 0; k < usedActionAssets.Count; k++)
            {
                if (usedActionAssets[k] == null)
                    continue;

                for (int m = 0; m < usedActionAssets[k].actionMaps.Count; m++)
                {
                    for (int j = 0; j < usedActionAssets[k].actionMaps[m].actions.Count; j++)
                    {
                        for (int i = 0; i < usedActionAssets[k].actionMaps[m].actions[j].bindings.Count; i++)
                        {
                            InputAction action = usedActionAssets[k].actionMaps[m].actions[j];
                            InputBinding binding = action.bindings[i];
                            InputStyleData data = GetStyleOpeningTag(action, binding, deviceString, controlSchemeName);
                            if (data == null)
                                continue;

                            outputList.Add(data);
                        }

                    }
                }
            }

            return outputList;
        }

        public static bool InputStyleDataListContainsBinding(List<InputStyleData> list, string bindingName)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].bindingName == bindingName)
                    return true;
            }
            return false;
        }

        //Overrides the values in the default style sheet with the ones currently in the style lists of the manager
        public static void OverrideStylesInStyleSheetDeviceSpecific(bool gamepadStyles)
        {
            List<TMP_InputStyleHack.StyleStruct> styleUpdates = GetStyleUpdatesDeviceSpecific(gamepadStyles);

            TMP_InputStyleHack.UpdateStyles(styleUpdates);
        }

        public static List<TMP_InputStyleHack.StyleStruct> GetStyleUpdatesDeviceSpecific(bool gamepadStyles)
        {

            List<TMP_InputStyleHack.StyleStruct> styleUpdates = new List<TMP_InputStyleHack.StyleStruct>();

            List<InputStyleData> inputStyles;

            string inputIconsOpeningTag = InputIconsManagerSO.Instance.openingTag;
            string inputIconsClosingTag = InputIconsManagerSO.Instance.closingTag;

            if (!gamepadStyles)
                inputStyles = InputIconsManagerSO.Instance.inputStyleKeyboardDataList;
            else
                inputStyles = InputIconsManagerSO.Instance.inputStyleGamepadDataList;

            for (int i = 0; i < inputStyles.Count; i++)
            {
                if (inputStyles[i] == null)
                    continue;


                string style = InputIconsManagerSO.Instance.GetCustomStyleTag(inputStyles[i]);
                style = inputIconsOpeningTag + style + inputIconsClosingTag;
                styleUpdates.Add(new TMP_InputStyleHack.StyleStruct(inputStyles[i].bindingName, style, ""));


                
                if (!InputIconsManagerSO.Instance.isUsingFonts) //if not using fonts, can skip this
                    continue;

                //handle font codes as well
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if(gamepadStyles)
                    iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();

                if (iconSet.fontAsset != null)
                    style = "<font=\"" + iconSet.fontAsset.name + "\"";
                   
                else
                {
                    style = "<font=\"InputIcons_Keyboard_Font SDF\"";
                    if (gamepadStyles)
                        style = "<font=\"InputIcons_Gamepad_Font SDF\"";
                }

                //if(material exists ...)
                //style += " material=\"InputIcons_Keyboard_Font SDF\"";
                style += ">";
                if (InputIconsManagerSO.Instance.showAllInputOptionsInStyles)
                    style += inputStyles[i].fontCode;
                else
                    style += inputStyles[i].fontCode_singleInput;

                string closingTag = "</font>";
                styleUpdates.Add(new TMP_InputStyleHack.StyleStruct("Font/" + inputStyles[i].bindingName, style, closingTag));
            }


            return styleUpdates;
        }



        //Needed in builds. Not necessarily needed in editor.
        public static void RefreshAllTMProUGUIObjects()
        {

            if(InputIconsManagerSO.Instance.textUpdateOptions == InputIconsManagerSO.TextUpdateOptions.SearchAndUpdate)
            {
                //go through all Text objects in the scene and set them dirty
                GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    TextMeshProUGUI[] tmpObjects = obj.GetComponentsInChildren<TextMeshProUGUI>();
                    foreach (TextMeshProUGUI tObj in tmpObjects)
                    {
                        tObj.SetAllDirty();
                    }

                    TextMeshPro[] tmpObjects2 = obj.GetComponentsInChildren<TextMeshPro>();
                    foreach (TextMeshPro tObj in tmpObjects2)
                    {
                        tObj.SetAllDirty();
                    }
                }
            }
            else if(InputIconsManagerSO.Instance.textUpdateOptions == InputIconsManagerSO.TextUpdateOptions.ViaInputIconsTextComponents)
            {
                InputIconsManagerSO.RefreshInputIconsTexts();
            }
        }
    }

}
