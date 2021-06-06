using System.Collections.Generic;
using NUnit.Framework;

public class ChooseCardToDrawTests
{
    [Test]
    public void ChooseCardToDraw()
    {
        var allCards = new Dictionary<int, CardTypeData>
        {
            {1, new InMemoryCard()},
            {2, new InMemoryCard()},
            {3, new InMemoryCard()},
        };
        var cardPlayZones = CardPlayZones.InMemory;
        cardPlayZones.TestInit();
        var context = EffectContext.ForTests(TestMembers.Any(), new Single(TestMembers.Any()), cardPlayZones, allCards);
        NextCardId.Reset();
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.ChooseCardToCreate,
            EffectScope = new StringReference("1,2,3"),
            Formula = "2",
        }, context);

        Assert.AreEqual(context.Selections.CardSelectionOptions.Length, 2);
        Assert.NotNull(context.Selections.OnCardSelected);
        Assert.AreEqual(2, NextCardId.Get());

        context.Selections.OnCardSelected(context.Selections.CardSelectionOptions[0]);

        Assert.AreEqual(context.PlayerCardZones.HandZone.Count, 1);
    }
}