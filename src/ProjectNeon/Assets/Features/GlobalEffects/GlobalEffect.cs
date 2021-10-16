using UnityEngine;

public abstract class GlobalEffect : ScriptableObject
{
    [SerializeField, ReadOnly] public int id;
    
    public abstract string ShortDescription { get; }
    public abstract string FullDescription { get; }
    public abstract void Apply();
}
