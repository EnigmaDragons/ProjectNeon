using UnityEngine;

public abstract class GlobalEffect : ScriptableObject
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    
    public abstract string ShortDescription { get; }
    public abstract string FullDescription { get; }
    public abstract void Apply();
    
    public override string ToString() => $"{id}{ShortDescription}";
    public override int GetHashCode() => ToString().GetHashCode();
    public override bool Equals(object other) 
        => other is GlobalEffect 
           && other.ToString().Equals(ToString());
}
