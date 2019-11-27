using NUnit.Framework;

public sealed class SpellFlatDamageEffectTests
{

    [Test]
    public void SpellFlatDamageEffect_DamageTargetWithNoResistance_DamageIsAppliedCorrectly()
    {
        Member attacker = TestMembers.Any();
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new SpellFlatDamageEffect(1).Apply(attacker, target);

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void SpellFlatDamageEffect_DamageTargetWithResistance_DamageIsAppliedCorrectly()
    {

        Member target = TestMembers.Create(
            s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f).With(StatType.Resistance, 0.5F)
        );

        new SpellFlatDamageEffect(2).Apply(TestMembers.Any(), target);

        Assert.AreEqual(
            9,
            target.State[TemporalStatType.HP]
        );
    }
}