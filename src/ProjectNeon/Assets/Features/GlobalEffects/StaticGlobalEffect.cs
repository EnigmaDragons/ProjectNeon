using UnityEngine;

[CreateAssetMenu(menuName = "GlobalEffects/GlobalEffect", fileName = "_", order = -100)]
public class StaticGlobalEffect : ScriptableObject, GlobalEffect, ILocalizeTerms
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private GlobalEffectData data;

    public string ShortDescriptionTerm => $"GlobalEffects/Effect{id}ShortDescription";
    public string FullDescriptionTerm => $"GlobalEffects/Effect{id}FullDescription";
    public void Apply(GlobalEffectContext ctx) => AllGlobalEffects.Apply(data, ctx);
    public void Revert(GlobalEffectContext ctx) => AllGlobalEffects.Revert(data, ctx);
    public GlobalEffectData Data => data.WithOriginatingId(id);
    
    public override string ToString() => $"{id}{data.ShortDescription}";

    public string[] GetLocalizeTerms()
        => new[] {ShortDescriptionTerm, FullDescriptionTerm};

    public override int GetHashCode() => ToString().GetHashCode();
    public override bool Equals(object other) 
        => other is StaticGlobalEffect 
           && other.ToString().Equals(ToString());
}
