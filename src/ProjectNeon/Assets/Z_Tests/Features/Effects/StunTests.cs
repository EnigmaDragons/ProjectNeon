using NUnit.Framework;

public sealed class StunTests
{
    [Test]
    public void Stun_ApplyEffect()
    {
        var stunFor5 = new EffectData { EffectType = EffectType.Stun, NumberOfTurns = new IntReference(5) };
        var target = TestMembers.Any();

        AllEffects.Apply(stunFor5, target, new Single(target));
        Assert.AreEqual(
            5,
            target.State[TemporalStatType.Stun]
        );
    }
}
