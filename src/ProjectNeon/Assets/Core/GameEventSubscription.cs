using System;

public sealed class GameEventSubscription
{
    public string EventType { get; }
    public Action<object> OnEvent { get; }
    public object Owner { get; }

    internal GameEventSubscription(string eventType, Action<object> onEvent, object owner)
    {
        EventType = eventType;
        OnEvent = onEvent;
        Owner = owner;
    }

    public static GameEventSubscription Create<T>(Action<T> onEvent, object owner)
    {
        return new GameEventSubscription(typeof(T).FullName, (o) => { if (o.GetType() == typeof(T)) onEvent((T)o); }, owner);
    }
}
