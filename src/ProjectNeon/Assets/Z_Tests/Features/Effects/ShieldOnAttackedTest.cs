using NUnit.Framework;
using UnityEngine;

[Ignore("Needs to be reworked.")]
public sealed class ShieldAttackedOnAttackTests
{
    private EffectData ChangeShieldOnAttackBy(float amount) => 
        new EffectData { 
            EffectType = EffectType.ShieldAttackedOnAttack, 
            FloatAmount = new FloatReference(amount) 
        };
    
    public ShieldAttackedOnAttackTests() => Debug.unityLogger.logEnabled = false;

    [Test]
    public void ShieldOnAttacked_ApplyEffect_TargetIsShieldedOnAttack()
    {
        var paladin = TestMembers.With(StatType.Toughness, 5);
        var ally = TestMembers.With(StatType.Toughness, 10);
        var attacker = TestMembers.Any();

        AllEffects.Apply(ChangeShieldOnAttackBy(1), paladin, new Single(ally));
        new Attack(0).Apply(attacker, new Single(ally));

        Assert.AreEqual(5, ally.State[TemporalStatType.Shield]);
    }

    [Test]
    public void ShieldOnAttacked_ApplyEffect_TargetIsNotShieldedIfNoAttacked()
    {
        var paladin = TestMembers.With(StatType.Toughness, 5);
        var ally = TestMembers.With(StatType.Toughness, 10);
        var attacker = TestMembers.Any();

        AllEffects.Apply(ChangeShieldOnAttackBy(1), paladin, new Single(ally));
        new Attack(0).Apply(attacker, new Single(paladin));

        Assert.AreEqual(0, ally.State[TemporalStatType.Shield]);
    }
}
