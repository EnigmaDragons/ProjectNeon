using NUnit.Framework;

public class ApplyTauntTests
{
    [Test]
    public void ApplyTaunt_HasTaunt()
    {
        var member = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustCounterFormula,
            Formula = "2",
            EffectScope = new StringReference("Taunt")
        }, member, member);
        
        Assert.AreEqual(2, member.State[TemporalStatType.Taunt]);
    }
}
