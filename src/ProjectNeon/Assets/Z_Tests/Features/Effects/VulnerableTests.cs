using NUnit.Framework;

public sealed class VulnerableTests
{
    private EffectData Vulnerable(int duration = 1) => new EffectData { EffectType = EffectType.ApplyVulnerable, DurationFormula = duration.ToString() };
    
    [Test]
    public void VulnerableMember_IsAttacked_TakesCorrectDamage()
    {
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 10));
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 30));
        
        TestEffects.Apply(Vulnerable(), attacker, target);
        
        TestEffects.ApplyBasicAttack(attacker, target);
        
        Assert.AreEqual(1.33f, target.State[StatType.Damagability]);
        Assert.AreEqual(16, target.CurrentHp());
    }
    
    [Test]
    public void VulnerableMember_ApplyVulnerableTwice_DoesNotStack()
    {
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 10));
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 30));
        
        TestEffects.Apply(Vulnerable(), attacker, target);
        TestEffects.Apply(Vulnerable(), attacker, target);
        
        TestEffects.ApplyBasicAttack(attacker, target);
        
        Assert.AreEqual(1.33f, target.State[StatType.Damagability]);
        Assert.AreEqual(16, target.CurrentHp());
    }
}
