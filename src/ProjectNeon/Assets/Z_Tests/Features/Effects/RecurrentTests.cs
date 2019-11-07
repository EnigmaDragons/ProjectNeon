using NUnit.Framework;

public sealed class RecurrentTests
{
    private EffectData DamageTarget(float amount) => new EffectData
    {
        EffectType = EffectType.PhysicalDamage,
        FloatAmount = new FloatReference(amount)
    };

    [Test]
    public void Recurrent_ApplyEffect_ApplyCorrectTimes()
    {
        Effect oneTimer = new Recurrent(
            AllEffects.Create(DamageTarget(1))
        );

        Member attacker = TestMembers.With(StatType.Attack, 1);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        oneTimer.Apply(attacker, new Single(target));
        oneTimer.Apply(attacker, new Single(target));

        Assert.AreEqual(
            9,
            target.State[TemporalStatType.HP],
            "Effect applied more than the repetition limit."
        );
    }
}
