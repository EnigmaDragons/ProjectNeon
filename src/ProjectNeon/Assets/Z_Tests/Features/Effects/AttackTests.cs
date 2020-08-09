using NUnit.Framework;

public sealed class AttackTests
{
    [Test]
    public void Attack_ApplyEffect_AttackIsAppliedCorrectly()
    {
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1f));
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));

        new Attack(1).Apply(attacker, new Single(target));

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
}
