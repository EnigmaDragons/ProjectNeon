using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public static class BattleEvent 
{
    private static readonly List<BattleEventSubscription> EventSubs = new List<BattleEventSubscription>();
    private static readonly BattleEvents BattleEvents = new BattleEvents();

    public static int SubscriptionCount => BattleEvents.SubscriptionCount;

    public static void Publish(object payload)
    {
        BattleEvents.Publish(payload);
        Debug.Log($"Publishing {payload.GetType()}");
    }

    public static void Subscribe<T>(Action<T> onEvent, object owner)
    {
        Subscribe(BattleEventSubscription.Create<T>(onEvent, owner));
    }

    public static void Subscribe(BattleEventSubscription subscription)
    {
        BattleEvents.Subscribe(subscription);
        EventSubs.Add(subscription);
    }

    public static void Unsubscribe(object owner)
    {
        BattleEvents.Unsubscribe(owner);
        EventSubs.Where(x => x.Owner.Equals(owner)).ForEach(x =>
        {
            EventSubs.Remove(x);
        });
    }
}
