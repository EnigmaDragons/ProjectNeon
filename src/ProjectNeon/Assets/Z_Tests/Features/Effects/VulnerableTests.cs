using NUnit.Framework;

public sealed class VulnerableTests
{
    [Test]
    public void VulnerableMember_IsAttacked_TakesCorrectDamage()
    {
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 10));
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 30));
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.ApplyVulnerable,
            NumberOfTurns = new IntReference(3)
        }, target, target);
        
        new Attack(1).Apply(attacker, new Single(target));
        
        Assert.AreEqual(1.33f, target.State[StatType.Damagability]);
        Assert.AreEqual(16, target.CurrentHp());
    }
}
