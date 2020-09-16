using System.Collections.Generic;
using NUnit.Framework;

public sealed class GainCreditsTests
{
    [Test]
    public void GainCredits_CreditsUpdated()
    {
        var adventureState = PartyAdventureState.InMemory();
        var effectContext = new EffectContext(TestMembers.Any(), new Single(TestMembers.Any()), adventureState, new PlayerState(), new Dictionary<int, Member>());
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.GainCredits,
            FloatAmount = new FloatReference(5)
        }, effectContext);
        
        Assert.AreEqual(5, adventureState.Credits);
    }
}
