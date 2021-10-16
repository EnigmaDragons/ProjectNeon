using UnityEngine;

[CreateAssetMenu(menuName = "GlobalEffects/TestEffect")]
public class TestGlobalEffect : StaticGlobalEffect
{
    public override string ShortDescription => "Test Effect";
    public override string FullDescription => "Test Effect";
    public override void Apply(GlobalEffectContext ctx) {}
    public override void Revert(GlobalEffectContext ctx) {}
}
