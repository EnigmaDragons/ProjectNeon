using NUnit.Framework;

public sealed class ForNumberOfTurnsTests
{
    private EffectData DamageTarget(float amount) => new EffectData {
        EffectType = EffectType.PhysicalDamage,
        FloatAmount = new FloatReference(amount)
    };

    [Test]
    public void ForNumberOfTurns_ApplyEffect_ApplyWhenActive()
    {
        ForNumberOfTurns timedDamage = new ForNumberOfTurns(
            AllEffects.Create(DamageTarget(1)), 2
        );

        Member attacker = TestMembers.With(StatType.Attack, 1);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        timedDamage.Apply(attacker, new Single(target));
        timedDamage.AdvanceTurn();
        timedDamage.Apply(attacker, new Single(target));
        
        Assert.AreEqual(
            8, 
            target.State[TemporalStatType.HP],
            "Effect did not applied when active."
        );
    }

    [Test]
    public void ForNumberOfTurns_ApplyEffect_DoesNotApplyWhenInactive()
    {
        ForNumberOfTurns timedDamage = new ForNumberOfTurns(AllEffects.Create(DamageTarget(1)));

        Member attacker = TestMembers.With(StatType.Attack, 1);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        timedDamage.Apply(attacker, new Single(target));
        timedDamage.AdvanceTurn();
        timedDamage.Apply(attacker, new Single(target));

        Assert.AreEqual(
            9,
            target.State[TemporalStatType.HP],
            "Effect did applied when inactive."
        );
    }
}
