using NUnit.Framework;

public sealed class SpellFlatDamageEffectTests
{

    [Test]
    public void SpellFlatDamageEffect_ApplyEffect_DamageIsAppliedCorrectly()
    {
        Member attacker = TestMembers.Any();
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new SpellFlatDamageEffect(1).Apply(attacker, target);

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
}