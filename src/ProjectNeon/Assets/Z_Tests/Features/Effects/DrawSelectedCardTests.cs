using System.Collections.Generic;
using NUnit.Framework;

public class DrawSelectedCardTests
{
    private Member _member = TestMembers.Any();
    private Card _deckCard;
    private Card _discardCard1;
    private Card _discardCard2;
    private CardPlayZones _cardPlayZones;
    private EffectContext _context;

    [SetUp]
    public void Init()
    {
        _deckCard = new Card(1, _member, new InMemoryCard());
        _discardCard1 = new Card(2, _member, new InMemoryCard());
        _discardCard2 = new Card(3, _member, new InMemoryCard());
        _cardPlayZones = CardPlayZones.InMemory;
        _cardPlayZones.TestInitFull(new [] { _deckCard }, new Card[0], new [] { _discardCard1, _discardCard2 });
        _context = EffectContext.ForTests(TestMembers.Any(), new Single(_member), _cardPlayZones, new Dictionary<int, CardTypeData>());
    }
    
    [Test]
    public void DrawFromHand()
    {
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.DrawSelectedCard,
            EffectScope = new StringReference(((int)CardLocation.Deck).ToString())
        }, _context);
        
        Assert.AreEqual(_context.Selections.CardSelectionOptions.Length, 1);
        Assert.NotNull(_context.Selections.OnCardSelected);

        _context.Selections.OnCardSelected(_deckCard);
        
        Assert.AreEqual(_context.PlayerCardZones.DrawZone.Count, 0);
        Assert.AreEqual(_context.PlayerCardZones.HandZone.Count, 1);
        Assert.AreEqual(_deckCard, _context.PlayerCardZones.HandZone.Cards[0]);
    }
    
    [Test]
    public void DrawFromDiscard()
    {
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.DrawSelectedCard,
            EffectScope = new StringReference(((int)CardLocation.Discard).ToString())
        }, _context);
        
        Assert.AreEqual(_context.Selections.CardSelectionOptions.Length, 2);
        Assert.NotNull(_context.Selections.OnCardSelected);

        _context.Selections.OnCardSelected(_discardCard1);
        
        Assert.AreEqual(_context.PlayerCardZones.DiscardZone.Count, 1);
        Assert.AreEqual(_context.PlayerCardZones.HandZone.Count, 1);
        Assert.AreEqual(_discardCard1, _context.PlayerCardZones.HandZone.Cards[0]);
    }
    
    [Test]
    public void DrawFromBoth()
    {
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.DrawSelectedCard,
            EffectScope = new StringReference("6")
        }, _context);
        
        Assert.AreEqual(_context.Selections.CardSelectionOptions.Length, 3);
        Assert.NotNull(_context.Selections.OnCardSelected);

        _context.Selections.OnCardSelected(_discardCard2);
        
        Assert.AreEqual(_context.PlayerCardZones.DiscardZone.Count, 1);
        Assert.AreEqual(_context.PlayerCardZones.HandZone.Count, 1);
        Assert.AreEqual(_discardCard2, _context.PlayerCardZones.HandZone.Cards[0]);
    }
}