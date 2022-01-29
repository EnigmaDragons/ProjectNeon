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
        
        var ctx = new CardSelectionContext(me, Maybe<Member>.Missing(), new[] {opp, me},
            new AIStrategy(Maybe<Member>.Missing(), new Single(opp), me, SpecialCards),
            PartyAdventureState.InMemory(), CardPlayZones.InMemory,
            new AiPreferences(), new CardTypeData[0], Maybe<CardTypeData>.Missing());
        
        AssertAlwaysPlays(ctx, DisabledCard);
    }
    
    [Test]
    public void GeneralAI_NonDesignatedAttackerWithRotatingPreference_RotatesCardTags()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();
        var designatedOtherAttacker = TestMembers.AnyEnemy();
        
        var ctx = new CardSelectionContext(me, Maybe<Member>.Missing(), new[] {opp, me, designatedOtherAttacker},
            new AIStrategy(Maybe<Member>.Missing(), new Single(opp), designatedOtherAttacker, SpecialCards),
            PartyAdventureState.InMemory(), CardPlayZones.InMemory,
            new AiPreferences {RotatePlayingCardTags = new[] {CardTag.Stealth, CardTag.Attack}}, new []{ StealthCard, AttackCard1}, Maybe<CardTypeData>.Missing());
        
        var played = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(StealthCard));
        Assert.AreEqual(AttackCard1.Name, played.Card.Name);
        
        var nextPlayed = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(AttackCard1));
        Assert.AreEqual(StealthCard.Name, nextPlayed.Card.Name);
    }
    
    [Test]
    public void GeneralAI_DesignatedAttackerWithRotatingPreference_OnlyPlaysAttackCards()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();
        
        var ctx = new CardSelectionContext(me, Maybe<Member>.Missing(), new[] {opp, me},
            new AIStrategy(Maybe<Member>.Missing(), new Single(opp), me, SpecialCards),
            PartyAdventureState.InMemory(), CardPlayZones.InMemory,
            new AiPreferences {RotatePlayingCardTags = new[] {CardTag.Stealth, CardTag.Attack}}, new []{ StealthCard, AttackCard1}, Maybe<CardTypeData>.Missing());
        
        AssertAlwaysPlays(ctx, AttackCard1);
    }
    
    [Test]
    public void GeneralAI_NonDesignatedAttackerPrefersNotToAttack_NeverAttacks()
    {
        var opp = TestMembers.Any();
        var me = TestMembers.AnyEnemy();
        var designatedOtherAttacker = TestMembers.AnyEnemy();
        var aiPreferences = new AiPreferences {UnpreferredCardTags = new[] {CardTag.Attack}};
        
        var ctx = new CardSelectionContext(me, Maybe<Member>.Missing(), new[] {opp, me, designatedOtherAttacker},
            new AIStrategy(Maybe<Member>.Missing(), new Single(opp), designatedOtherAttacker, SpecialCards),
            PartyAdventureState.InMemory(), CardPlayZones.InMemory,
            aiPreferences, new []{ ControlCard, AttackCard1 }, Maybe<CardTypeData>.Missing());
        
        AssertAlwaysPlays(ctx, ControlCard);
    }

    private void AssertAlwaysPlays(CardSelectionContext ctx, CardTypeData c)
    {
        var lastPlayedCard = Maybe<CardTypeData>.Missing();
        for (var i = 0; i < 10; i++)
        {
            var played = ExecuteGeneralAiSelection(ctx.WithLastPlayedCard(lastPlayedCard));
            Assert.AreEqual(c.Name, played.Card.Name);
            lastPlayedCard = played.Card;
        }
    }

    private static PlayedCardV2 ExecuteGeneralAiSelection(CardSelectionContext ctx)
    {
        return ctx.WithCommonSenseSelections()
            .WithSelectedFocusCardIfApplicable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithFinalizedSmartCardSelection()
            .WithSelectedTargetsPlayedCard();
    }

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