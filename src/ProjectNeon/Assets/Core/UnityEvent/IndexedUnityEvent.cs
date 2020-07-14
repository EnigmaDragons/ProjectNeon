using System;
using UnityEngine.Events;

[Serializable]
public struct IndexedUnityEvent
{
    public UnityEvent Action;
    public int Index;
}