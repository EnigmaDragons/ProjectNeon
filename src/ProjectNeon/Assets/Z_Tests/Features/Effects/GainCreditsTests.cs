using System.Collections.Generic;
using NUnit.Framework;

public sealed class GainCreditsTests
{
    [Test]
    public void GainCredits_CreditsUpdated()
    {
        var adventureState = PartyAdventureState.InMemory();
        var effectContext = new EffectContext(TestMembers.Any(), new Single(TestMembers.Any()), Maybe<Card>.Missing(), ResourceQuantity.None, adventureState, 
            new PlayerState(0), new TeamState[0], new Dictionary<int, Member>(), CardPlayZones.InMemory);
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.GainCredits,
            FloatAmount = new FloatReference(5)
        }, effectContext);
        
        Assert.AreEqual(5, adventureState.Credits);
    }
}
