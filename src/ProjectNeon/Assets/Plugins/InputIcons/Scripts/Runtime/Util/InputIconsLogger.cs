using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputIcons
{
    public static class InputIconsLogger
    {

       

        public static void Log(string message)
        {
            if (InputIconsManagerSO.Instance.loggingEnabled)
                Debug.Log("Input Icons: "+ message);
        }

        public static void Log(string message, Object context)
        {
            if (InputIconsManagerSO.Instance.loggingEnabled)
                Debug.Log("Input Icons: "+ message, context);
        }

        public static void LogWarning(string message)
        {
            if (InputIconsManagerSO.Instance.loggingEnabled)
                Debug.LogWarning("Input Icons: " + message);
        }

        public static void LogWarning(string message, Object context)
        {
            if (InputIconsManagerSO.Instance.loggingEnabled)
                Debug.LogWarning("Input Icons: " + message, context);
        }

        public static void LogError(string message)
        {
            if (InputIconsManagerSO.Instance.loggingEnabled)
                Debug.LogError("Input Icons: " + message);
        }

        public static void LogError(string message, Object context)
        {
            if (InputIconsManagerSO.Instance.loggingEnabled)
                Debug.LogError("Input Icons: " + message, context);
        }
    }
}

