using NUnit.Framework;

[TestFixture]
public class AegisTests
{
    [TestCase(TemporalStatType.Disabled)]
    [TestCase(TemporalStatType.Blind)]
    [TestCase(TemporalStatType.CardStun)]
    [TestCase(TemporalStatType.Inhibit)]
    [TestCase(TemporalStatType.Confused)]
    public void Aegis_GiveNegativeCounters_Applied(TemporalStatType statType)
    {
        var defender = DefenderWithAegis();
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(AdjustCounterEffect(statType, "1"), attacker, defender);
        
        Assert.AreEqual(0, defender.State[statType]);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }
    
    [TestCase(TemporalStatType.Disabled)]
    [TestCase(TemporalStatType.Blind)]
    [TestCase(TemporalStatType.CardStun)]
    [TestCase(TemporalStatType.Inhibit)]
    [TestCase(TemporalStatType.Confused)]
    public void Aegis_RemoveNegativeCounters_NotApplied(TemporalStatType statType)
    {
        var defender = DefenderWithAegis();
        defender.Apply(m => m.Adjust(statType, 1));
        
        TestEffects.Apply(AdjustCounterEffect(statType, "-1"), defender, defender);
        
        Assert.AreEqual(0, defender.State[statType]);
        Assert.AreEqual(1, defender.State[TemporalStatType.Aegis]);
    }
    
    [TestCase(TemporalStatType.Stealth)]
    [TestCase(TemporalStatType.DoubleDamage)]
    [TestCase(TemporalStatType.Dodge)]
    [TestCase(TemporalStatType.Taunt)]
    [TestCase(TemporalStatType.Lifesteal)]
    [TestCase(TemporalStatType.Evade)]
    [TestCase(TemporalStatType.Spellshield)]
    public void Aegis_TakePositiveCounters_Applied(TemporalStatType statType)
    {
        var defender = DefenderWithAegis();
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(AdjustCounterEffect(statType, "-1"), attacker, defender);
        
        Assert.AreEqual(0, defender.State[statType]);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }
    
    [TestCase(TemporalStatType.Stealth)]
    [TestCase(TemporalStatType.DoubleDamage)]
    [TestCase(TemporalStatType.Dodge)]
    [TestCase(TemporalStatType.Taunt)]
    [TestCase(TemporalStatType.Lifesteal)]
    [TestCase(TemporalStatType.Evade)]
    [TestCase(TemporalStatType.Spellshield)]
    public void Aegis_GivePositiveCounters_NotApplied(TemporalStatType statType)
    {
        var defender = DefenderWithAegis();
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(AdjustCounterEffect(statType, "1"), attacker, defender);
        
        Assert.AreEqual(1, defender.State[statType]);
        Assert.AreEqual(1, defender.State[TemporalStatType.Aegis]);
    }

    [Test]
    public void Aegis_DamageOverTime_Applied()
    {
        var defender = DefenderWithAegis();
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.DamageOverTimeFormula, 
            Formula = "2", 
            NumberOfTurns = new IntReference(3)
        }, attacker, defender);
        
        Assert.AreEqual(0, defender.State.StatusesOfType(StatusTag.DamageOverTime).Length);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }

    [TestCase(EffectType.AdjustStatAdditivelyFormula, 3)]
    [TestCase(EffectType.AdjustStatMultiplicativelyFormula, 2)]
    public void Aegis_StatBuff_AegisNotApplicable(EffectType effectType, int expectedAttack)
    {
        var member = TestMembers.Create(s => s.With(StatType.Attack, 1));
        member.Apply(m => m.Adjust(TemporalStatType.Aegis, 1));
        
        TestEffects.Apply(new EffectData
        {
            EffectType = effectType,
            Formula = "2",
            EffectScope = new StringReference(StatType.Attack.ToString()),
            NumberOfTurns = new IntReference(1)
        }, member, member);
        
        Assert.AreEqual(expectedAttack, member.Attack());
        Assert.AreEqual(1, member.State[TemporalStatType.Aegis]);
    }
    
    [Test]
    public void Aegis_AdditiveStatDebuff_Applied()
    {
        var member = TestMembers.Create(s => s.With(StatType.Attack, 2));
        member.Apply(m => m.Adjust(TemporalStatType.Aegis, 1));
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustStatAdditivelyFormula,
            Formula = "-1",
            EffectScope = new StringReference(StatType.Attack.ToString()),
            NumberOfTurns = new IntReference(1)
        }, member, member);
        
        Assert.AreEqual(2, member.Attack());
        Assert.AreEqual(0, member.State[TemporalStatType.Aegis]);
    }
    
    [Test]
    public void Aegis_MultiplicativeStatDebuff_Applied()
    {
        var member = TestMembers.Create(s => s.With(StatType.Attack, 2));
        member.Apply(m => m.Adjust(TemporalStatType.Aegis, 1));
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustStatMultiplicativelyFormula,
            Formula = "0.5",
            EffectScope = new StringReference(StatType.Attack.ToString()),
            NumberOfTurns = new IntReference(1)
        }, member, member);
        
        Assert.AreEqual(2, member.Attack());
        Assert.AreEqual(0, member.State[TemporalStatType.Aegis]);
    }

    [Test]
    public void Aegis_RemoveAllShields_Applied()
    {
        var defender = DefenderWithAegis();
        defender.Apply(m => m.AdjustShield(10));
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(new EffectData { EffectType = EffectType.ShieldRemoveAll }, attacker, defender);
        
        Assert.AreEqual(10, defender.CurrentShield());
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }
    
    [Test]
    public void Aegis_ReduceShields_Applied()
    {
        var defender = DefenderWithAegis();
        defender.Apply(m => m.AdjustShield(10));
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ShieldFormula,
            Formula = "-1"
        }, attacker, defender);
        
        Assert.AreEqual(10, defender.CurrentShield());
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }

    [Test]
    public void Aegis_TransferNegativePrimaryResource_Applied()
    {
        var defender = DefenderWithAegis();
        defender.Apply(d => d.AdjustPrimaryResource(2));
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.TransferPrimaryResourceFormula,
            Formula = "-1"
        }, attacker, defender);
        
        Assert.AreEqual(2, defender.PrimaryResource().Amount);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }

    [Test]
    public void Aegis_ResourceLossFlat_Applied()
    {
        var defender = DefenderWithAegis();
        defender.Apply(d => d.AdjustPrimaryResource(2));
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustResourceFlat,
            BaseAmount = new IntReference(-1),
            EffectScope = new StringReference(defender.PrimaryResource().ResourceType)
        }, attacker, defender);
        
        Assert.AreEqual(2, defender.PrimaryResource().Amount);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }
    
    [Test]
    public void Aegis_PrimaryResourceLoss_Applied()
    {
        var defender = DefenderWithAegis();
        defender.Apply(d => d.AdjustPrimaryResource(2));
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustPrimaryResource,
            BaseAmount = new IntReference(-1)
        }, attacker, defender);
        
        Assert.AreEqual(2, defender.PrimaryResource().Amount);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }
    
    [Test]
    public void Aegis_PrimaryResourceFormulaLoss_Applied()
    {
        var defender = DefenderWithAegis();
        defender.Apply(d => d.AdjustPrimaryResource(2));
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustPrimaryResourceFormula,
            Formula = "-1"
        }, attacker, defender);
        
        Assert.AreEqual(2, defender.PrimaryResource().Amount);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }
    
    private EffectData AdjustCounterEffect(TemporalStatType statType, string formulaAmount)
        => new EffectData
            {
                EffectType = EffectType.AdjustCounterFormula,
                EffectScope = new StringReference(statType.ToString()),
                Formula = formulaAmount
            };

    private Member DefenderWithAegis(int numOfAegis = 1)
    {
        var defender = TestMembers.Create(s => s
            .With(StatType.MaxShield, 20)
            .With(new InMemoryResourceType("Ammo") { MaxAmount = 6 }));
        defender.Apply(m => m.Adjust(TemporalStatType.Aegis, numOfAegis));
        return defender;
    }
}