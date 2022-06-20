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
}
