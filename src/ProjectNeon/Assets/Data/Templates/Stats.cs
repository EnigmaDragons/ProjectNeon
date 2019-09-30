using UnityEngine;

public abstract class Stats : ScriptableObject, IStats
{
    public abstract int MaxHP { get; }
    public abstract int MaxShield { get; }
    public abstract int Attack { get; }
    public abstract int Magic { get; }
    public abstract float Armor { get; }
    public abstract float Resistance { get; }
    public abstract IResourceType[] ResourceTypes { get; }
}
