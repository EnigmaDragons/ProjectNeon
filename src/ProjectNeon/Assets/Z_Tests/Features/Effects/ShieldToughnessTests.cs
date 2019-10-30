using NUnit.Framework;

[TestFixture]
public sealed class ShieldToughnessTests
{
    [Test]
    public void ShieldToughness_ApplyEffect()
    {
        var shieldTarget = new EffectData { EffectType = EffectType.ShieldToughness, FloatAmount = new FloatReference(0.2F)};
        var performer = TestMembers.With(StatType.Toughness, 3);
        var target = TestMembers.With(StatType.Toughness, 10);
        
        AllEffects.Apply(shieldTarget, performer, new MemberAsTarget(target));
        Assert.AreEqual(5, target.State[TemporalStatType.Shield]);
    }
}
