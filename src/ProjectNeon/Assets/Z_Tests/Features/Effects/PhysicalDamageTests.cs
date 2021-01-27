using NUnit.Framework;

public sealed class PhysicalDamageTests
{
    private readonly Member _attacker = TestMembers.Create(s => s.With(StatType.Attack, 2));

    [Test]
    public void PhysicalDamage_DamageUnarmoredTarget_ApplyEffect()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        TestEffects.ApplyBasicAttack(_attacker, target);

        Assert.AreEqual(8, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void PhysicalDamage_DamageArmoredTarget_ApplyEffect()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Armor, 1));

        TestEffects.ApplyBasicAttack(_attacker, target);

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
    
    [Test]
    public void PhysicalDamage_DamageMultipleEnemies_ApplyEffect()
    {
        var targets = new[]
        {
            TestMembers.Create(s => s.With(StatType.MaxHP, 10)),
            TestMembers.Create(s => s.With(StatType.MaxHP, 10))
        };
        
        TestEffects.Apply(TestEffects.BasicAttack, _attacker, new Multiple(targets));

        Assert.AreEqual(8, targets[0].CurrentHp());
        Assert.AreEqual(8, targets[1].CurrentHp());
    }
}
