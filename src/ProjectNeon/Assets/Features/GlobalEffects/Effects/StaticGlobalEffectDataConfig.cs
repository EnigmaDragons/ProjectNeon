using UnityEngine;

[CreateAssetMenu(menuName = "GlobalEffects/GlobalEffect", fileName = "_", order = -100)]
public class StaticGlobalEffectDataConfig : StaticGlobalEffect
{
    [SerializeField] private GlobalEffectData data;

    protected override GlobalEffectData SourceData => data;
    public override string ShortDescription => data.ShortDescription;
    public override string FullDescription => data.FullDescription;

    public override void Apply(GlobalEffectContext ctx) => AllGlobalEffects.Apply(data, ctx);
    public override void Revert(GlobalEffectContext ctx) => AllGlobalEffects.Revert(data, ctx);
}
