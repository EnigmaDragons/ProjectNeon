using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
    private IEnumerable<GameEventListener> listeners = Array.Empty<GameEventListener>();

    public void Publish()
    {
        listeners.ForEach(x => x.OnEventRaised());
    }

    public void RegisterListener(GameEventListener listener)
    {
        listeners = listeners.Concat(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        listeners = listeners.Except(listener.AsArray());
    }
}
