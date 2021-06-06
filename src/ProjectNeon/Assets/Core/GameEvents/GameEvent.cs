using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Game Event")]
[Obsolete] 
public class GameEvent : ScriptableObject
{
    private IEnumerable<GameEventSubscription> listeners = Array.Empty<GameEventSubscription>();

    public void Publish() => CleansedListeners.CopiedForEach(l => l.OnEvent(l));

    public void Subscribe(Action action, object subscriber) => Subscribe(new GameEventSubscription(name, x => action(), subscriber));
    public void Subscribe(GameEventListener listener) => Subscribe(new GameEventSubscription(name, x => listener.OnEventRaised(), listener));
    public void Unsubscribe(GameEventListener listener) => Unsubscribe((object) listener);
    public void Subscribe(GameEventSubscription e) => listeners = CleansedListeners.Concat(e);
    public void Unsubscribe(object owner) => listeners = CleansedListeners.Where(l => !ReferenceEquals(l.Owner, owner));

    private IEnumerable<GameEventSubscription> CleansedListeners => listeners.Where(x => x.Owner != null);
    
    public static GameEvent InMemory
    {
        get
        {
            var e = (GameEvent) FormatterServices.GetUninitializedObject(typeof(GameEvent));
            e.listeners = Array.Empty<GameEventSubscription>();
            return e;
        }
    }
}
