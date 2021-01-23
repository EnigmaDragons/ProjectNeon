using NUnit.Framework;

public sealed class PhysicalDamageTests
{
    private readonly Member _attacker = TestMembers.Create(s => s.With(StatType.Attack, 2));

    [Test]
    public void PhysicalDamage_DamageUnarmoredTarget_ApplyEffect()
    {
        var attacker = TestMembers.With(StatType.Attack, 2);
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new DealDamage(new PhysicalDamage(1)).Apply(attacker, new MemberAsTarget(target), Maybe<Card>.Missing());

        Assert.AreEqual(8, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void PhysicalDamage_DamageArmoredTarget_ApplyEffect()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Armor, 1));

        new DealDamage(new PhysicalDamage(1)).Apply(_attacker, new Single(target), Maybe<Card>.Missing());

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
    
    [Test]
    public void PhysicalDamage_DamageMultipleEnemies_ApplyEffect()
    {
        var target = new[]
        {
            TestMembers.Create(s => s.With(StatType.MaxHP, 10)),
            TestMembers.Create(s => s.With(StatType.MaxHP, 10))
        };

        new DealDamage(new PhysicalDamage(1)).Apply(_attacker, new Multiple(target), Maybe<Card>.Missing());

        Assert.AreEqual(8, target[0].State[TemporalStatType.HP]);
        Assert.AreEqual(8, target[1].State[TemporalStatType.HP]);
    }
}
