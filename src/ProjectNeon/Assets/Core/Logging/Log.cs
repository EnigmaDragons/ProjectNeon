using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Log
{
    private static readonly List<Action<string>> _additionalMessageSinks = new List<Action<string>>();

    public static void AddSink(Action<string> sink) => _additionalMessageSinks.Add(sink);

    public static void Info(string msg)
    {
        #if UNITY_EDITOR && !HIGH_PERF
        try
        {
            _additionalMessageSinks.ForEach(s => s(msg));
            Debug.Log(msg);;
        }
        catch
        {
        }
        #endif
    }

    public static void Info(string msg, Object context)
    {
        #if UNITY_EDITOR && !HIGH_PERF
        try
        {
            _additionalMessageSinks.ForEach(s => s(msg));
            Debug.Log(msg, context);
        }
        catch
        {
        }
        #endif
    }

    public static void Warn(string msg)
    {
        #if !HIGH_PERF
        try
        {
            _additionalMessageSinks.ForEach(s => s(msg));
            Debug.LogWarning(msg);
        }
        catch
        {
        }
        #endif
    }

    public static void Error(string msg)
    {
        try
        {
            _additionalMessageSinks.ForEach(s => s(msg));
            Debug.LogError(msg);
        }
        catch
        {
        }
    }

    public static void Error(string msg, Object context)
    {
        try
        {
            _additionalMessageSinks.ForEach(s => s(msg));
            Debug.LogError(msg, context);
        }
        catch
        {
        }
    }

    public static void Error(Exception e)
    {
        try
        {
            _additionalMessageSinks.ForEach(s => s(e.ToString()));
            Debug.LogException(e);
        }
        catch
        {
        }
    }

    public static void NonCrashingError(Exception e)
    {
        #if !HIGH_PERF
        try
        {
            _additionalMessageSinks.ForEach(s => s(e.ToString()));
            Debug.LogException(new Exception("Non-Crashing", e));
        }
        catch
        {
        }
        #endif
    }

    public static void NonCrashingError(string msg)
    {
        #if !HIGH_PERF
        try
        {
            _additionalMessageSinks.ForEach(s => s(msg));
            Debug.LogError("Non-Crashing Error: " + msg);
        }
        catch
        {
        }
        #endif
    }

    public static void InfoOrError(string msg, bool isError)
    {
        if (isError)
            Error(msg);
        else
            Info(msg);
    }

    public static void InfoOrWarn(string msg, bool isWarn)
    {
        if (isWarn)
            Warn(msg);
        else
            Info(msg);
    }
    
    public static void ErrorIfNull<T>(T obj, string context, string elementName)
    {
        if (obj == null)
            Error($"{context}: {elementName} is null/not assigned");
    }
    
    public static void ErrorIfNull(Object obj, string context, string elementName)
    {
        if (obj == null)
            Error($"{context}: {elementName} is null/not assigned");
    }

    public static T InfoLogged<T>(this T value)
    {
        Info(value.ToString());
        return value;
    }
    
    public static T LogIfNull<T>(T thing, string context, T defaultThing)
    {
        if (thing != null)
            return thing;
        NonCrashingError($"{typeof(T).Name} is null: {context}");
        return defaultThing;
    }
    
    public static T LogIfNull<T>(T thing, string context)
    {
        if (thing == null)
            Error($"{typeof(T).Name} is null: {context}");
        return thing;
    }
}
