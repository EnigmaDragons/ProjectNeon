using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
    private IEnumerable<GameEventSubscription> listeners = Array.Empty<GameEventSubscription>();

    public void Publish() => CleansedListeners.ForEach(l => l.OnEvent(l));

    public void Subscribe(Action action, object subscriber) => Subscribe(new GameEventSubscription(name, x => action(), subscriber));
    public void Subscribe(GameEventListener listener) => Subscribe(new GameEventSubscription(name, x => listener.OnEventRaised(), listener));
    public void Unsubscribe(GameEventListener listener) => Unsubscribe((object) listener);
    public void Subscribe(GameEventSubscription e) => listeners = CleansedListeners.Concat(e);
    public void Unsubscribe(object owner) => listeners = CleansedListeners.Where(l => !ReferenceEquals(l.Owner, owner));

    private IEnumerable<GameEventSubscription> CleansedListeners => listeners.Where(x => x.Owner != null);
}
