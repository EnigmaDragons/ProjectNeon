using NUnit.Framework;

public sealed class HealFlatTests
{
    [Test]
    public void HealFlat_ApplyEffect_DoesNotPassFullHealth()
    {
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new Heal(5).Apply(TestMembers.Any(), target);
        
        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void HealFlat_Take6DamageAndThenHeal5_HpIsCorrect()
    {
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        target.State.TakeRawDamage(6);

        new Heal(5).Apply(TestMembers.Any(), target);

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
}
