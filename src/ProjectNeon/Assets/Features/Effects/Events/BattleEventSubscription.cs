using UnityEngine;
using UnityEditor;
using System;

public sealed class BattleEventSubscription : ScriptableObject
{
    public Type EventType { get; }
    public Action<object> OnEvent { get; }
    public object Owner { get; }

    internal BattleEventSubscription(Type eventType, Action<object> onEvent, object owner)
    {
        EventType = eventType;
        OnEvent = onEvent;
        Owner = owner;
    }

    public static BattleEventSubscription Create<T>(Action<T> onEvent, object owner)
    {
        return new BattleEventSubscription(typeof(T), (o) => { if (o.GetType() == typeof(T)) onEvent((T)o); }, owner);
    }
}