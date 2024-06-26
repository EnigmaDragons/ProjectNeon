using NUnit.Framework;

public class DoubleDamageTests
{
    [Test]
    public void DoubleDamage_AttackDamageIsDoubled()
    {
        var attacker = TestMembers.With(StatType.Attack, 2);
        var target = TestMembers.With(StatType.MaxHP, 10);
        
        attacker.State.Adjust(TemporalStatType.DoubleDamage, 1);

        TestEffects.Apply(TestEffects.BasicAttack, attacker, target);
        
        Assert.AreEqual(0, attacker.State[TemporalStatType.DoubleDamage]);
        Assert.AreEqual(6, target.CurrentHp());
    }
    
    [Test]
    public void DoubleDamage_DamageSpellIsDoubled()
    {
        var attacker = TestMembers.With(StatType.Magic, 2);
        var target = TestMembers.With(StatType.MaxHP, 10);
        
        attacker.State.Adjust(TemporalStatType.DoubleDamage, 1);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.MagicAttackFormula, 
            Formula = "1 * Magic"
        }, attacker, target);
        
        Assert.AreEqual(0, attacker.State[TemporalStatType.DoubleDamage]);
        Assert.AreEqual(6, target.CurrentHp());
    }
    
    [Test]
    public void DoubleDamage_TrueDamageAttackIsDoubled()
    {
        var attacker = TestMembers.With(StatType.Attack, 2);
        var target = TestMembers.With(StatType.MaxHP, 10);
        
        attacker.State.Adjust(TemporalStatType.DoubleDamage, 1);

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.TrueDamageAttackFormula,
            Formula = "1 * Attack"
        }, attacker, target);
        
        Assert.AreEqual(0, attacker.State[TemporalStatType.DoubleDamage]);
        Assert.AreEqual(6, target.CurrentHp());
    }
}
