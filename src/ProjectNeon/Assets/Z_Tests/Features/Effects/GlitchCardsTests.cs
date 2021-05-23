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
        var effectContext = new EffectContext(TestMembers.Any(), new Single(member1), Maybe<Card>.Missing(), ResourceQuantity.None, PartyAdventureState.InMemory(), 
            new PlayerState(0), new Dictionary<int, Member>(), cardPlayZones, new UnpreventableContext(), new SelectionContext());
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.GlitchRandomCards,
            BaseAmount = new IntReference(1),
            EffectScope = new StringReference("1")
        }, effectContext);
        
        Assert.AreEqual(CardMode.Glitched, member1HandCard1.Mode);
    }
}