using System;

public sealed class MessageSubscription
{
    public Type EventType { get; }
    public Action<object> OnEvent { get; }
    public object Owner { get; }

    internal MessageSubscription(Type eventType, Action<object> onEvent, object owner)
    {
        EventType = eventType;
        OnEvent = onEvent;
        Owner = owner;
    }

    public static MessageSubscription Create<T>(Action<T> onEvent, object owner) 
        => new MessageSubscription(typeof(T), (o) => { if (o.GetType() == typeof(T)) onEvent((T)o); }, owner);
}
