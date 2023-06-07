#if STEAMWORKS_NET
using Steamworks;
using System;
#endif

using UnityEngine;

namespace InputIcons
{
    public class InputIconsSteamworksExtensionSO : ScriptableObject
    {
#if STEAMWORKS_NET
       private void Awake()
       {
           InputIconSetConfiguratorSO.onIconSetUpdated += HandleIconSetUpdated;
           SteamAPI.Init();
       }

        private void OnDestroy()
        {
            InputIconSetConfiguratorSO.onIconSetUpdated -= HandleIconSetUpdated;
        }

        public void HandleIconSetUpdated()
       {

           if (!SteamAPI.IsSteamRunning())
           {
               return;
           }

           InputIconSetBasicSO usedIconSet =  InputIconSetConfiguratorSO.GetCurrentIconSet();
           if(usedIconSet is InputIconSetKeyboardSO)
           {
               return;
           }

           //use Steamworks helper methods to detect device used
           //Debug.Log("used gamepad: " + usedIconSet.deviceDisplayName);
           try
           {
               SteamInput.Init(false);
               SteamInput.RunFrame(); 
           }
           catch (InvalidOperationException)
           {
               //steam input not initialized, steam might not running. Can not override current icon set.
               //Debug.Log("Steam input not initialized");
               return; 
           }

           InputHandle_t[] inputHandles = new InputHandle_t[Constants.STEAM_INPUT_MAX_COUNT];
           int result = SteamInput.GetConnectedControllers( inputHandles );
           ESteamInputType inputType = SteamInput.GetInputTypeForHandle(inputHandles[0]);


           //Debug.Log("detected steam input: "+ inputType.ToString());
           string chosenInput = "";
           if (inputType == ESteamInputType.k_ESteamInputType_XBox360Controller)
           {
               chosenInput = "steam override: xbox 360 icons SET";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.xBoxIconSet);

           }
           else if (inputType == ESteamInputType.k_ESteamInputType_XBoxOneController)
           {
               chosenInput =  "steam override: xbox one icons SET";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.xBoxIconSet);

           }
           else if (inputType == ESteamInputType.k_ESteamInputType_PS3Controller)
           {
               chosenInput = "steam override: ps3 icons SET";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.ps3IconSet);

           }
           else if (inputType == ESteamInputType.k_ESteamInputType_PS4Controller)
           {
               chosenInput = "steam override: ps4 icons SET";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.ps4IconSet);

           }
           else if (inputType == ESteamInputType.k_ESteamInputType_PS5Controller)
           {
               chosenInput = "steam override: ps5 icons SET";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.ps5IconSet);

           }
           else if (inputType == ESteamInputType.k_ESteamInputType_SwitchProController)
           {
               chosenInput = "steam override: switch pro icons SET";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.switchIconSet);

           }
           else if (inputType == ESteamInputType.k_ESteamInputType_SteamController)
           {
               chosenInput = "steam override: steam controller icons SET (xbox layout)";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.xBoxIconSet);

           }
           else if (inputType == ESteamInputType.k_ESteamInputType_SteamDeckController)
           {
               chosenInput = "steam override: steam DECK controller icons SET (xbox layout)";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.xBoxIconSet);

           }
           else if (inputType == ESteamInputType.k_ESteamInputType_GenericGamepad)
           {
               chosenInput = "steam override: generic XBox back icons SET";
               InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.xBoxIconSet);
           }

            if (InputIconSetConfiguratorSO.Instance.overwriteIconSet != null
                 && chosenInput != "")
            {
                chosenInput = "steam override: overwrite gamepad icons SET";
                InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.overwriteIconSet);
            }
             


        }
#endif
    }
}