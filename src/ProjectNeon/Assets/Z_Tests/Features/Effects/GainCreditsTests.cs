using System.Collections.Generic;
using NUnit.Framework;

public sealed class GainCreditsTests
{
    [Test]
    public void GainCredits_CreditsUpdated()
    {
        var adventureState = PartyAdventureState.InMemory();
        var effectContext = new EffectContext(TestMembers.Any(), new Single(TestMembers.Any()), Maybe<Card>.Missing(), ResourceQuantity.None, adventureState, 
            new PlayerState(0), new Dictionary<int, Member>(), CardPlayZones.InMemory, new UnpreventableContext(), new SelectionContext(), new Dictionary<int, CardTypeData>(), 0, 0);
        
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
        
        var effectContext = new EffectContext(TestMembers.Any(), new Single(TestMembers.Any()), Maybe<Card>.Missing(), ResourceQuantity.None, adventureState, 
            new PlayerState(0), new Dictionary<int, Member>(), CardPlayZones.InMemory, new UnpreventableContext(), new SelectionContext(), new Dictionary<int, CardTypeData>(), 0, 0);
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.GainCredits,
            FloatAmount = new FloatReference(-20)
        }, effectContext);
        
        Assert.AreEqual(0, adventureState.Credits);
    }
}
