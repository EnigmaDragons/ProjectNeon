using UnityEngine;

public abstract class StaticGlobalEffect : ScriptableObject, GlobalEffect
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    
    public abstract string ShortDescription { get; }
    public abstract string FullDescription { get; }
    public abstract void Apply(GlobalEffectContext ctx);
    public abstract void Revert(GlobalEffectContext ctx);
    
    public override string ToString() => $"{id}{ShortDescription}";
    public override int GetHashCode() => ToString().GetHashCode();
    public override bool Equals(object other) 
        => other is StaticGlobalEffect 
           && other.ToString().Equals(ToString());
}
