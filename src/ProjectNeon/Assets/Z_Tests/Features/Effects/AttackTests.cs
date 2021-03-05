using NUnit.Framework;

public sealed class AttackTests
{
    [Test]
    public void Attack_ApplyEffect_AttackIsAppliedCorrectly()
    {
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1f));
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        
        TestEffects.Apply(TestEffects.BasicAttack, attacker, target);

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
    
    [Test]
    public void Attack_ArmorIsHigherThanAttack_HpIsCorrect()
    {
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1f));
        var target = TestMembers.Create(s => s.With(StatType.Damagability, 1).With(StatType.MaxHP, 10).With(StatType.Armor, 2));
        target.State.SetHp(8);

        TestEffects.Apply(TestEffects.BasicAttack, attacker, target);
        
        Assert.AreEqual(8, target.State[TemporalStatType.HP]);
    }
}
