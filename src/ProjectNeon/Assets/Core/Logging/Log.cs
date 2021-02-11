using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Log
{
    public static void Info(string msg) => IgnoreExceptions(() => Debug.Log(msg));
    public static void Info(string msg, Object context) => IgnoreExceptions(() => Debug.Log(msg, context));
    public static void Info(Object obj) => IgnoreExceptions(() => Debug.Log(obj));
    public static void Warn(string msg) => IgnoreExceptions(() => Debug.LogWarning(msg));
    public static void Error(string msg) => IgnoreExceptions(() => Debug.LogError(msg));
    public static void Error(string msg, Object context) => IgnoreExceptions(() => Debug.LogError(msg, context));
    public static void Error(Exception e) => IgnoreExceptions(() => Debug.LogException(e));

    private static void IgnoreExceptions(Action a)
    {
        try
        {
            a();
        }
        catch (Exception e)
        {
            
        }
    }
}
