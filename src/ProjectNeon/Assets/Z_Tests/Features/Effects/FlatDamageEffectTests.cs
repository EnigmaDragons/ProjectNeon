using NUnit.Framework;

public sealed class FlatDamageEffectTests
{

    [Test]
    public void FlatDamageEffect_ApplyEffect_DamageIsAppliedCorrectly()
    {
        Member attacker = TestMembers.Any();
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new FlatDamageEffect(1).Apply(attacker, target);

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
}