using NUnit.Framework;

public sealed class OnNextTurnEffectTests

{
    private EffectData DamageTarget(float amount) => new EffectData {
        EffectType = EffectType.PhysicalDamage,
        FloatAmount = new FloatReference(amount)
    };

    [Test]
    public void OnNextTurnEffect_ApplyEffect_ApplyOnNextTurn()
    {
        OnNextTurnEffect timedDamage = new OnNextTurnEffect(
            AllEffects.Create(DamageTarget(1))
        );

        Member attacker = TestMembers.With(StatType.Attack, 1);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        Message.Publish(new TurnEnd());
        timedDamage.Apply(attacker, new Single(target));
        
        Assert.AreEqual(
            9, 
            target.State[TemporalStatType.HP],
            "Effect did not applied on next turn."
        );
    }

    [Test]
    public void OnNextTurnEffect_ApplyEffect_DoesNotApplyInCurrentTurn()
    {
        OnNextTurnEffect timedDamage = new OnNextTurnEffect(
            AllEffects.Create(DamageTarget(1))
        );

        Member attacker = TestMembers.With(StatType.Attack, 1);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        timedDamage.Apply(attacker, new Single(target));

        Assert.AreEqual(
            10,
            target.State[TemporalStatType.HP],
            "Effect applied on current turn."
        );
    }
}
