using System.Collections.Generic;
using NUnit.Framework;

public class AiCardSelectionTests
{
    private static int _nextCardId = 1;
    private static readonly CardTypeData DisabledCard = CreateCard("DisabledCard", new CardTag[0], Scope.One, Group.Self);
    private static readonly CardTypeData AntiStealthCard = CreateCard("AntiStealthCard", new[] {CardTag.AntiStealth}, Scope.All, Group.Opponent);
    private static readonly CardTypeData AIGlitchedCard = CreateCard("AIGlitchedCard", new CardTag[0], Scope.One, Group.Self);
    private static readonly CardTypeData StealthCard = CreateCard("Stealth", CardTag.Stealth.AsArray(), Scope.One, Group.Self);
    private static readonly CardTypeData ControlCard = CreateCard("Control 1", CardTag.Disable.AsArray(), Scope.One, Group.Opponent);
    private static readonly CardTypeData AttackCard1 = CreateCard("Attack 1", CardTag.Attack.AsArray(), Scope.One, Group.Opponent);
    private static readonly CardTypeData AttackCard2 = CreateCard("Attack 2", CardTag.Attack.AsArray(), Scope.One, Group.Opponent);
    private static readonly EnemySpecialCircumstanceCards SpecialCards = new EnemySpecialCircumstanceCards(DisabledCard, AntiStealthCard, AIGlitchedCard);
    
    [Test]
    public void GeneralAI_NoCards_AlwaysPlaysDisabledCard()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();

        var ctx = AsDesignatedAttacker(me, opp, new AiPreferences());
        
        AssertAlwaysPlays(ctx, DisabledCard);
    }

    [Test]
    public void GeneralAI_TwoEquallyGoodCards_AlternatesCards()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();

        var ctx = AsDesignatedAttacker(me, opp, new AiPreferences { CardOrderPreferenceFactor = 0 }, AttackCard2, AttackCard1);

        var ai = CreateAI();
        var played = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(AttackCard2), ai, 1);
        Assert.AreEqual(AttackCard1.Name, played.Card.Name);
        
        var nextPlayed = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(AttackCard1), ai, 2);
        Assert.AreEqual(AttackCard2.Name, nextPlayed.Card.Name);
    }
    
    [Test]
    public void GeneralAI_CardOrderFactor_PlaysPreferredCardFirst()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();

        var ctx = AsDesignatedAttacker(me, opp, new AiPreferences { CardOrderPreferenceFactor = 1 }, AttackCard2, AttackCard1);

        AssertFirstCardIsAlways(ctx, AttackCard2);
    }
    
    [Test]
    public void GeneralAI_NonDesignatedAttackerWithRotatingPreference_RotatesCardTags()
    {
        var ai = CreateAI();
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();
        var designatedOtherAttacker = TestMembers.AnyEnemy();
        
        var ctx = AsNonDesignatedAttacker(me, opp, designatedOtherAttacker,
            new AiPreferences {RotatePlayingCardTags = new[] {CardTag.Stealth, CardTag.Attack}}, 
            StealthCard, AttackCard1);

        var played = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(StealthCard), ai, 1);
        Assert.AreEqual(AttackCard1.Name, played.Card.Name);
        
        var nextPlayed = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(AttackCard1), ai, 2);
        Assert.AreEqual(StealthCard.Name, nextPlayed.Card.Name);
    }
    
    [Test]
    public void GeneralAI_DesignatedAttackerWithRotatingPreference_OnlyPlaysAttackCards()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();

        var ctx = AsDesignatedAttacker(me, opp, new AiPreferences {RotatePlayingCardTags = new[] {CardTag.Stealth, CardTag.Attack}}, StealthCard, AttackCard1);

        AssertAlwaysPlays(ctx, AttackCard1);
    }
    
    [Test]
    public void GeneralAI_NonDesignatedAttackerPrefersNotToAttack_NeverAttacks()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();
        var designatedOtherAttacker = TestMembers.AnyEnemy();
        var aiPreferences = new AiPreferences {UnpreferredCardTags = new[] {CardTag.Attack}};

        var ctx = AsNonDesignatedAttacker(me, opp, designatedOtherAttacker, aiPreferences, AttackCard1, ControlCard);
        
        var played = ExecuteGeneralAiSelection(ctx, CreateAI(), 1);
        Assert.AreNotEqual(AttackCard1.Name, played.Card.Name);
    }

    [Test]
    public void GeneralAI_DoesntPlayADamageOverTimeDefenseOnALowHpAlly()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.CreateEnemy(x => x.With(StatType.MaxHP, 4));
        var designatedOtherAttacker = TestMembers.CreateEnemy(x => x.With(StatType.MaxHP, 4));
        var dotDefense = CreateCard("Dot Defense", new[] {CardTag.Defense, CardTag.DamageOverTime}, Scope.One, Group.Ally);

        var ctx = AsNonDesignatedAttacker(me, opp, designatedOtherAttacker, new AiPreferences(), dotDefense, AttackCard1);
        
        AssertAlwaysPlays(ctx, AttackCard1);
    }

    [Test]
    public void GeneralAI_HasExclusiveCardsAndMultiplePlaysPerTurn_OnlyPlaysOneExclusiveCard()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();
        var designatedOtherAttacker = TestMembers.AnyEnemy();
        var aiPreferences = new AiPreferences { CardTagPriority = CardTag.Exclusive.AsArray() };
        var exclusiveCard1 = CreateCard("Exclusive 1", new[] {CardTag.Exclusive}, Scope.One, Group.Opponent);
        var exclusiveCard2 = CreateCard("Exclusive 2", new[] {CardTag.Exclusive}, Scope.One, Group.Opponent);
        
        var ctx = new CardSelectionContext(me, Maybe<Member>.Missing(), new[] {opp, me, designatedOtherAttacker},
            new AIStrategy(Maybe<Member>.Missing(), new Single(opp), designatedOtherAttacker, SpecialCards),
            PartyAdventureState.InMemory(), CardPlayZones.InMemory,
            aiPreferences, Maybe<CardTypeData>.Missing(), new []{ exclusiveCard1, exclusiveCard2, AttackCard1 }, new CardTypeData[0]);

        var ai = CreateAI();
        var played = ExecuteGeneralAiSelection(ctx, ai, 1);
        Assert.AreNotEqual(AttackCard1.Name, played.Card.Name);
        
        var nextPlayed = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(played.Card.Type), ai, 1);
        Assert.AreEqual(AttackCard1.Name, nextPlayed.Card.Name);
    }
    
    [Test]
    public void GeneralAI_WithUnhighlightedCard_NeverPlaysIt()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();
        var designatedOtherAttacker = TestMembers.AnyEnemy();
        
        var ctx = new CardSelectionContext(me, Maybe<Member>.Missing(), new[] {opp, me, designatedOtherAttacker},
            new AIStrategy(Maybe<Member>.Missing(), new Single(opp), designatedOtherAttacker, SpecialCards),
            PartyAdventureState.InMemory(), CardPlayZones.InMemory,
            new AiPreferences(), Maybe<CardTypeData>.Missing(), new []{ AttackCard1, AttackCard2 }, AttackCard2.AsArray());

        AssertAlwaysPlays(ctx, AttackCard1);
    }

    [Test]
    public void GeneralAI_MultiCardDesignatedAttacker_OnlyNeedsToPlayOneAttack()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.CreateEnemy(s => s.With(StatType.ExtraCardPlays, 2));

        var ctx = AsDesignatedAttacker(me, opp, new AiPreferences(), ControlCard, AttackCard1)
            .WithLastPlayedCard(AttackCard1)
            .WithCurrentTurnPlayedCards(AttackCard1.AsArray());

        for (var i = 0; i < 10; i++)
        {
            var ai = CreateAI();
            var cardTwo = ExecuteGeneralAiSelection(ctx, ai);
            Assert.AreEqual(ControlCard.Name, cardTwo.Card.Name, $"Wrong Card Played on Turn {i + 1}");
        }
    }
    
    private static GeneralAI CreateAI() 
    {
        var ai = TestableObjectFactory.Create<GeneralAI>();
        ai.InitForBattle();
        return ai;
    }
    
    private static CardSelectionContext AsDesignatedAttacker(Member me, Member opp, AiPreferences aiPreferences, params CardTypeData[] cards)
        => new CardSelectionContext(me, Maybe<Member>.Missing(), new[] {opp, me},
            new AIStrategy(Maybe<Member>.Missing(), new Single(opp), me, SpecialCards),
            PartyAdventureState.InMemory(), CardPlayZones.InMemory, aiPreferences,Maybe<CardTypeData>.Missing(), cards, new CardTypeData[0]);
    
    private static CardSelectionContext AsNonDesignatedAttacker(Member me, Member opp, Member designatedAttacker, AiPreferences aiPreferences, params CardTypeData[] cards)
        => new CardSelectionContext(me, Maybe<Member>.Missing(), new[] {opp, me, designatedAttacker},
            new AIStrategy(Maybe<Member>.Missing(), new Single(opp), designatedAttacker, SpecialCards),
            PartyAdventureState.InMemory(), CardPlayZones.InMemory, aiPreferences, Maybe<CardTypeData>.Missing(), cards, new CardTypeData[0]);

    private void AssertAlwaysPlays(CardSelectionContext ctx, CardTypeData c) => AssertAlwaysPlays(ctx, c, CreateAI());
    private void AssertAlwaysPlays(CardSelectionContext ctx, CardTypeData c, GeneralAI ai)
    {
        var lastPlayedCard = Maybe<CardTypeData>.Missing();
        for (var i = 0; i < 10; i++)
        {
            var played = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(lastPlayedCard), ai, i + 1);
            Assert.AreEqual(c.Name, played.Card.Name, $"Wrong Card Played on Turn {i + 1}");
            lastPlayedCard = played.Card;
        }
    }

    private void AssertFirstCardIsAlways(CardSelectionContext ctx, CardTypeData c)
    {
        for (var i = 0; i < 10; i++)
        {
            var played = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(Maybe<CardTypeData>.Missing()), CreateAI(), i + 1);
            Assert.AreEqual(c.Name, played.Card.Name, $"Wrong Card Played on Turn {i + 1}");
        }
    }

    private static IPlayedCard ExecuteGeneralAiSelection(CardSelectionContext ctx, GeneralAI ai, int turnNumber = 0) 
        => ai.Play(turnNumber, ctx);

    private static CardTypeData CreateCard(string name, CardTag[] tags, Scope s, Group g)
    {
       return new InMemoryCard
        {
            Name = name,
            Id = _nextCardId++,
            Tags = new HashSet<CardTag>(tags),
            ActionSequences = new []
            {
                CardActionSequence.Create(s, g, TestableObjectFactory.Create<CardActionsData>(), false),
            }
        };
    }
}
