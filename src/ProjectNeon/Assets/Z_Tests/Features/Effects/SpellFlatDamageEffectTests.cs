using NUnit.Framework;

public sealed class SpellFlatDamageEffectTests
{
    [Test]
    public void SpellFlatDamageEffect_DamageTargetWithNoResistance_DamageIsAppliedCorrectly()
    {
        var attacker = TestMembers.Any();
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));

        new SpellFlatDamageEffect(1).Apply(attacker, target, Maybe<Card>.Missing());

        Assert.AreEqual(9, target.CurrentHp());
    }

    [Test]
    public void SpellFlatDamageEffect_DamageTargetWithResistance_DamageIsAppliedCorrectly()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Resistance, 1));

        new SpellFlatDamageEffect(2).Apply(TestMembers.Any(), target, Maybe<Card>.Missing());

        Assert.AreEqual(9, target.CurrentHp());
    }
}
