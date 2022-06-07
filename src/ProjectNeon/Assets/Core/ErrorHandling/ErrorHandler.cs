using System;
using UnityEngine;

public static class ErrorHandler
{
    private static Action<string> _onUnhandledError = _ => { };
    
    private static bool _isInitialized;
    public static void Init()
    {
        if (_isInitialized)
            return;
        
        _isInitialized = true;
        Application.logMessageReceived += HandleException;
    }

    public static void SetErrorAction(Action<string> onError) => _onUnhandledError = onError;
     
    private static void HandleException(string condition, string stackTrace, LogType type)
    {
// #if UNITY_EDITOR
//         if (!stackTrace.Contains("error CS"))
//             return;
// #endif
        if (type == LogType.Exception || type == LogType.Error)
            _onUnhandledError(type + "\n" + condition + "\n" + stackTrace);
    }

    public static void BasicNeverCrash(Action action)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }
}
