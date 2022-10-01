using NUnit.Framework;

public class AdjustCounterTests
{
    [Test]
    public void AdjustCounterFormula_WithManyAegises_AegisPreventsCounterReduction()
    {
        var member = TestMembers.Any();
        member.State.Adjust(TemporalStatType.Aegis, 5);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustCounterFormula,
            Formula = "-99",
            EffectScope = new StringReference("Aegis"),
            Unpreventable = false
        }, member, member);
        
        Assert.AreEqual(4, member.State[TemporalStatType.Aegis]);
    }
    
    [Test]
    public void AdjustCounterFormula_WithManyAegises_DoesNotPreventUnpreventableEffect()
    {
        var member = TestMembers.Any();
        member.State.Adjust(TemporalStatType.Aegis, 5);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustCounterFormula,
            Formula = "-99",
            EffectScope = new StringReference("Aegis"),
            Unpreventable = true
        }, member, member);
        
        Assert.AreEqual(0, member.State[TemporalStatType.Aegis]);
    }
    
    [Test]
    public void AdjustCounterFormula_RemoveProminentWithActiveAegis_NotPrevented()
    {
        var member = TestMembers.Any();
        member.State.Adjust(TemporalStatType.Aegis, 5);
        member.State.Adjust(TemporalStatType.Prominent, 1);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustCounterFormula,
            Formula = "-99",
            EffectScope = new StringReference("Prominent"),
            Unpreventable = false
        }, member, member);
        
        Assert.AreEqual(0, member.State[TemporalStatType.Prominent]);
        Assert.AreEqual(5, member.State[TemporalStatType.Aegis]);
    }
    
    [Test]
    public void AdjustCounterFormula_AddProminentWithActiveAegis_NotPrevented()
    {
        var member = TestMembers.Any();
        member.State.Adjust(TemporalStatType.Aegis, 5);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustCounterFormula,
            Formula = "1",
            EffectScope = new StringReference("Prominent"),
            Unpreventable = false
        }, member, member);
        
        Assert.AreEqual(1, member.State[TemporalStatType.Prominent]);
        Assert.AreEqual(5, member.State[TemporalStatType.Aegis]);
    }
}
