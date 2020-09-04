using NUnit.Framework;

public class ApplyTauntTests
{
    [Test]
    public void ApplyTaunt_HasTaunt()
    {
        var member = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ApplyTaunt, 
            NumberOfTurns = new IntReference(2)
        }, member, member);
        
        Assert.AreEqual(2, member.State[TemporalStatType.Taunt]);
    }
}
