using System.Collections.Generic;
using NUnit.Framework;

public sealed class GainCreditsTests
{
    [Test]
    public void GainCredits_CreditsUpdated()
    {
        var adventureState = PartyAdventureState.InMemory();
        var effectContext = EffectContext.ForTests(TestMembers.Any(), new Single(TestMembers.Any()), adventureState);
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.GainCredits,
            FloatAmount = new FloatReference(5)
        }, effectContext);
        
        Assert.AreEqual(5, adventureState.Credits);
    }
    
    [Test]
    public void GainNegativeCredits_DoesntGoBelowZero()
    {
        var adventureState = PartyAdventureState.InMemory();
        adventureState.UpdateCreditsBy(10);
        
        var effectContext = EffectContext.ForTests(TestMembers.Any(), new Single(TestMembers.Any()), adventureState);
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.GainCredits,
            FloatAmount = new FloatReference(-20)
        }, effectContext);
        
        Assert.AreEqual(0, adventureState.Credits);
    }
}
