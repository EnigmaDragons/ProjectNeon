using System;
using UnityEngine.Events;

[Serializable]
public sealed class NamedCommand
{
    public string Name;
    public UnityEvent Action;
}
