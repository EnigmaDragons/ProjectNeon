using NUnit.Framework;


public sealed class PhysicalDamageTests
{
    private Member attacker = TestMembers.With(StatType.Attack, 2);

    [Test]
    public void PhysicalDamage_DamageUnarmoredTarget_ApplyEffect()
    {
        Member attacker = TestMembers.With(StatType.Attack, 2);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new DamageApplied(new PhysicalDamage(1)).Apply(attacker, new MemberAsTarget(target));

        Assert.AreEqual(
            8,
            target.State[TemporalStatType.HP]
        );
    }

    [Test]
    public void PhysicalDamage_DamageArmoredTarget_ApplyEffect()
    {
        
        Member target = TestMembers.Create(
            s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f).With(StatType.Armor, 0.5F)
        );

        new DamageApplied(new PhysicalDamage(1)).Apply(attacker, new MemberAsTarget(target));

        Assert.AreEqual(
            9,
            target.State[TemporalStatType.HP]
        );
    }
}
