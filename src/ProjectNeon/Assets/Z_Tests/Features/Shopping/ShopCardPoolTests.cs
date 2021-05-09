using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class ShopCardPoolTests
{
    private const string _archetype1 = "archetype1";
    private const string _archetype2 = "archetype2";
    private const string _archetype3 = "archetype3";
    private readonly CardType _starterArchetype1 = TestableObjectFactory.Create<CardType>().Initialized(Rarity.Starter);
    private readonly CardType _commonArchetype2 = TestableObjectFactory.Create<CardType>().Initialized(Rarity.Common);
    private readonly CardType _uncommonArchetype2 = TestableObjectFactory.Create<CardType>().Initialized(Rarity.Uncommon);
    private readonly CardType _rareNeutral = TestableObjectFactory.Create<CardType>().Initialized(Rarity.Rare);
    private readonly CardType _epicArchetype1And2 = TestableObjectFactory.Create<CardType>().Initialized(Rarity.Epic);
    private readonly CardType _rareArchetype3 = TestableObjectFactory.Create<CardType>().Initialized(Rarity.Rare);
    private ShopCardPool _cardPool;

    [SetUp]
    public void Setup()
    {
        var starterArchetype1Pool = TestableObjectFactory.Create<ShopCardPool>()
            .Initialized(new [] {_archetype1}, new [] {Rarity.Starter}, new ShopCardPool[0], new []{_starterArchetype1});
        var commonUncommonArchetype2Pool = TestableObjectFactory.Create<ShopCardPool>()
            .Initialized(new [] {_archetype2}, new [] {Rarity.Common, Rarity.Rare}, new ShopCardPool[0], new []{_commonArchetype2, _uncommonArchetype2});
        var neutralPool = TestableObjectFactory.Create<ShopCardPool>()
            .Initialized(new string[0], new [] {Rarity.Rare}, new ShopCardPool[0], new []{_rareNeutral});
        var archetype1And2Pool = TestableObjectFactory.Create<ShopCardPool>()
            .Initialized(new [] {_archetype1, _archetype2}, new [] {Rarity.Epic}, new ShopCardPool[0], new []{_epicArchetype1And2});
        var archetype3Pool = TestableObjectFactory.Create<ShopCardPool>()
            .Initialized(new [] {_archetype3}, new [] {Rarity.Rare}, new ShopCardPool[0], new []{_rareArchetype3});
        _cardPool = TestableObjectFactory.Create<ShopCardPool>()
            .Initialized(new string[0], new Rarity[0], new []{starterArchetype1Pool, commonUncommonArchetype2Pool, neutralPool, archetype1And2Pool, archetype3Pool}, new CardType[0]);
    }
    
    [Test]
    public void NoSpecifiedArgs_ReturnsAllCards()
    {
        var cards = _cardPool.Get(new HashSet<string>()).ToArray();
        Assert.AreEqual(6, cards.Length);
        Assert.Contains(_starterArchetype1, cards);
        Assert.Contains(_commonArchetype2, cards);
        Assert.Contains(_uncommonArchetype2, cards);
        Assert.Contains(_rareNeutral, cards);
        Assert.Contains(_epicArchetype1And2, cards);
        Assert.Contains(_rareArchetype3, cards);
    }

    [Test]
    public void SpecifiedArchetype_ReturnsListWithThatArchetypeAndNeutralCard()
    {
        var cards = _cardPool.Get(new HashSet<string> {_archetype1}).ToArray();
        Assert.AreEqual(2, cards.Length);
        Assert.Contains(_starterArchetype1, cards);
        Assert.Contains(_rareNeutral, cards);
    }

    [Test]
    public void SpecifiedRarity_ReturnsListWithThatRarity()
    {
        var cards = _cardPool.Get(new HashSet<string>(), Rarity.Starter).ToArray();
        Assert.AreEqual(1, cards.Length);
        Assert.Contains(_starterArchetype1, cards);
    }

    [Test]
    public void SpecifiedRarity_ReturnsPartialListFromListWithThatAsOneOfTheRarities()
    {
        var cards = _cardPool.Get(new HashSet<string>(), Rarity.Common).ToArray();
        Assert.AreEqual(1, cards.Length);
        Assert.Contains(_commonArchetype2, cards);
    }

    [Test]
    public void SpecifiedMultipleArchetypes_ReturnsAllApplicableLists()
    {
        var cards = _cardPool.Get(new HashSet<string> {_archetype1, _archetype2}).ToArray();
        Assert.AreEqual(5, cards.Length);
        Assert.Contains(_starterArchetype1, cards);
        Assert.Contains(_commonArchetype2, cards);
        Assert.Contains(_uncommonArchetype2, cards);
        Assert.Contains(_rareNeutral, cards);
        Assert.Contains(_epicArchetype1And2, cards);
    }
}