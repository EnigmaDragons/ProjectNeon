using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Log
{
    private static readonly List<Action<string>> _additionalMessageSinks = new List<Action<string>>();

    public static void AddSink(Action<string> sink) => _additionalMessageSinks.Add(sink);

    public static void Info(string msg) => IgnoreExceptions(() => SinkAnd(msg, () => Debug.Log(msg)));
    public static void Info(string msg, Object context) => IgnoreExceptions(() => SinkAnd(msg, () => Debug.Log(msg, context)));
    public static void Info(Object obj) => IgnoreExceptions(() => Debug.Log(obj));
    public static void Warn(string msg) => IgnoreExceptions(() => SinkAnd("Warn: " + msg, () => Debug.LogWarning(msg)));
    public static void Error(string msg) => IgnoreExceptions(() => SinkAnd("Error: " + msg, () => Debug.LogError(msg)));
    public static void Error(string msg, Object context) => IgnoreExceptions(() => SinkAnd("Error: " + msg, () => Debug.LogError(msg, context)));
    public static void Error(Exception e) => IgnoreExceptions(() => Debug.LogException(e));

    private static void SinkAnd(string msg, Action a)
    {
        _additionalMessageSinks.ForEach(s => s(msg));
        a();
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
