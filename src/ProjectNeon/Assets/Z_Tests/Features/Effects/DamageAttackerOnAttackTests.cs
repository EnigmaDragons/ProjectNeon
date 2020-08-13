using NUnit.Framework;
using UnityEngine;

[Ignore("Needs to be reworked.")]
public sealed class DamageAttackerOnAttackTests
{
    private EffectData DamageAttackerOnAttack(float amount) => 
        new EffectData { 
            EffectType = EffectType.DamageAttackerOnAttack, 
            FloatAmount = new FloatReference(amount) 
        };

    public DamageAttackerOnAttackTests() => Debug.logger.logEnabled = false;
    
    [Test]
    public void DamageOnAttacker_ApplyEffect_AttackerIsDamagedOnAttack()
    {
        var paladin = TestMembers.Create(s => s.With(StatType.Attack, 1f));
        var ally = TestMembers.Any();
        var attacker = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        AllEffects.Apply(DamageAttackerOnAttack(1), paladin, new Single(ally));
        new Attack(1).Apply(attacker, new Single(ally));

        Assert.AreEqual(9, attacker.State[TemporalStatType.HP]);
    }

    [Test]
    public void DamageOnAttacker_ApplyEffect_AttackerIsNotDamagedOnAttackingOther()
    {
        var paladin = TestMembers.Create(s => s.With(StatType.Attack, 1f));
        var ally = TestMembers.Any();
        var attacker = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        AllEffects.Apply(DamageAttackerOnAttack(1), paladin, new Single(ally));
        new Attack(1).Apply(attacker, new Single(paladin));

        Assert.AreEqual(10, attacker.State[TemporalStatType.HP]);
    }
}
