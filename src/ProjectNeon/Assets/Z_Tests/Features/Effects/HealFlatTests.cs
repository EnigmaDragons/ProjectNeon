using NUnit.Framework;

public sealed class HealFlatTests
{
    [Test]
    public void HealFlat_ApplyEffect_DoesNotPassFullHealth()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));

        new Heal(5).Apply(TestMembers.Any(), target);
        
        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void HealFlat_Take6DamageAndThenHeal5_HpIsCorrect()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        target.State.TakeRawDamage(6);

        new Heal(5).Apply(TestMembers.Any(), target);

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void HealFlat_OnUnconsciousMember_ShouldDoNothing()
    {
        var target = TestMembers.Any();
        target.State.TakeRawDamage(99);
        
        new Heal(5).Apply(TestMembers.Any(), target);
        
        Assert.IsFalse(target.IsConscious());
    }
}
