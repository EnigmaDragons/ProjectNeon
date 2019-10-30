﻿using NUnit.Framework;

[TestFixture]
public sealed class ShieldToughnessTests
{
    [Test]
    public void ShieldToughness_ApplyEffect()
    {
        var shieldTarget = new EffectData { EffectType = EffectType.ShieldToughness, FloatAmount = new FloatReference(1)};
        var performer = TestMembers.With(StatType.Toughness, 5);
        var target = TestMembers.With(StatType.Toughness, 10);
        
        AllEffects.Apply(shieldTarget, performer, new MemberAsTarget(target));
        Assert.AreEqual(5, target.State[TemporalStatType.Shield]);
    }
}
