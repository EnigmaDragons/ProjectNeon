using NUnit.Framework;

public sealed class AttackTests
{
    
    [Test]
    public void Attack_ApplyEffect_AttackIsAppliedCorrectly()
    {
        Member attacker = TestMembers.Create(
            s => s.With(StatType.Attack, 1f).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new Attack(1).Apply(attacker, new Single(attacker));

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
}
