using UnityEngine;

public static class Log
{
    public static void Info(string msg)
    {
#if UNITY_EDITOR
        Debug.Log(msg);
#endif
    }
    
    public static void Info(string msg, Object context)
    {
#if UNITY_EDITOR
        Debug.Log(msg, context);
#endif
    }
    
    public static void Info(Object obj)
    {
#if UNITY_EDITOR
        Debug.Log(obj);
#endif
    }

    public static void Error(string msg)
    {
#if UNITY_EDITOR
        Debug.LogError(msg);
#endif
    }

    public static void Error(string msg, Object context)
    {
#if UNITY_EDITOR
        Debug.LogError(msg, context);
#endif
    }

    public static void Warn(string msg)
    {
#if UNITY_EDITOR
        Debug.LogWarning(msg);
#endif
    }
}
