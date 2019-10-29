using System;
using System.Collections.Generic;
using System.Linq;

public sealed class BattleEvents
{
    private readonly Dictionary<Type, List<object>> _eventActions = new Dictionary<Type, List<object>>();
    private readonly Dictionary<object, List<BattleEventSubscription>> _ownerSubscriptions = new Dictionary<object, List<BattleEventSubscription>>();

    public int SubscriptionCount => _eventActions.Sum(e => e.Value.Count);

    public void Publish(object payload)
    {
        var eventType = payload.GetType();

        if (_eventActions.ContainsKey(eventType))
            foreach (var action in _eventActions[eventType].ToList())
                ((Action<object>)action)(payload);
    }

    public void Subscribe(BattleEventSubscription subscription)
    {
        var eventType = subscription.EventType;
        if (!_eventActions.ContainsKey(eventType))
            _eventActions[eventType] = new List<object>();
        if (!_ownerSubscriptions.ContainsKey(subscription.Owner))
            _ownerSubscriptions[subscription.Owner] = new List<BattleEventSubscription>();
        _eventActions[eventType].Add(subscription.OnEvent);
        _ownerSubscriptions[subscription.Owner].Add(subscription);
    }

    public void Unsubscribe(object owner)
    {
        if (!_ownerSubscriptions.ContainsKey(owner))
            return;
        var events = _ownerSubscriptions[owner];
        for (var i = 0; i < _eventActions.Count; i++)
            _eventActions.ElementAt(i).Value.RemoveAll(x => events.Any(y => y.OnEvent.Equals(x)));
        _ownerSubscriptions.Remove(owner);
    }
}
}