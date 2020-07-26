using NUnit.Framework;


public sealed class PhysicalDamageTests
{
    private Member attacker = TestMembers.With(StatType.Attack, 2);

    [Test]
    public void PhysicalDamage_DamageUnarmoredTarget_ApplyEffect()
    {
        Member attacker = TestMembers.With(StatType.Attack, 2);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new Damage(new PhysicalDamage(1)).Apply(attacker, new MemberAsTarget(target));

        Assert.AreEqual(
            8,
            target.State[TemporalStatType.HP]
        );
    }

    [Test]
    public void PhysicalDamage_DamageArmoredTarget_ApplyEffect()
    {
        
        Member target = TestMembers.Create(
            s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f).With(StatType.Armor, 50F)
        );

        new Damage(new PhysicalDamage(1)).Apply(attacker, new Single(target));

        Assert.AreEqual(
            9,
            target.State[TemporalStatType.HP]
        );
    }
    
    [Test]
    public void PhysicalDamage_DamageMultipleEnemies_ApplyEffect()
    {
        Member[] target = new Member[]
        {
            TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f).With(StatType.Armor, 0F)),
            TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f).With(StatType.Armor, 0F))
        };

        new Damage(new PhysicalDamage(1)).Apply(attacker, new Multiple(target));

        Assert.AreEqual(8, target[0].State[TemporalStatType.HP]);
        Assert.AreEqual(8, target[1].State[TemporalStatType.HP]);
    }
}
