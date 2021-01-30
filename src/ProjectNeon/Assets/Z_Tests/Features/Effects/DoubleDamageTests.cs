using NUnit.Framework;

public class DoubleDamageTests
{
    [Test]
    public void DoubleDamage_AttackDamageIsDoubled()
    {
        var attacker = TestMembers.With(StatType.Attack, 2);
        var target = TestMembers.With(StatType.MaxHP, 10);
        
        attacker.State.Adjust(TemporalStatType.DoubleDamage, 1);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.Attack, 
            FloatAmount = new FloatReference(1)
        }, attacker, target);
        
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
            EffectType = EffectType.MagicAttack, 
            FloatAmount = new FloatReference(1)
        }, attacker, target);
        
        Assert.AreEqual(0, attacker.State[TemporalStatType.DoubleDamage]);
        Assert.AreEqual(6, target.CurrentHp());
    }
}
