using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

public sealed class GlitchCardsTests
{
    [Test]
    public void CardIsGlitched()
    {
        var member1 = TestMembers.Any();
        var member1HandCard1 = new Card(1, member1, new InMemoryCard());
        var cardPlayZones = CardPlayZones.InMemory;
        cardPlayZones.TestInit(member1HandCard1);
        var effectContext = EffectContext.ForTests(TestMembers.Any(), new Single(member1), cardPlayZones, new Dictionary<int, CardTypeData>());
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.GlitchRandomCards,
            BaseAmount = new IntReference(1),
            EffectScope = new StringReference("1")
        }, effectContext);
        
        Assert.AreEqual(CardMode.Glitched, member1HandCard1.Mode);
    }
}