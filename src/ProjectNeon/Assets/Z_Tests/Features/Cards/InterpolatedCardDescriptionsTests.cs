using System;
using System.Linq;
using NUnit.Framework;

public sealed class InterpolatedCardDescriptionsTests
{
    private readonly Member Owner = new Member(0, "", "", TeamType.Enemies, 
        new StatAddends()
            .With(StatType.Damagability, 1)
            .With(StatType.Attack, 8)
            .With(new InMemoryResourceType("Ammo") { MaxAmount = 6, StartingAmount = 6}), 
        BattleRole.Unknown);
    
    private readonly EffectData BasicAttack = new EffectData
    {
        EffectType = EffectType.Attack,
        FloatAmount = new FloatReference(1)
    };
    
    [Test]
    public void Interpolated_AttackCard_IsCorrect() 
        => AssertMatchesIgnoreStyling("Deal 8", Description("Deal {E[0]}", BasicAttack, Owner));

    [Test]
    public void Interpolated_AttackWithoutOwner_IsCorrect() 
        => AssertMatchesIgnoreStyling("1x ATK", ForEffect(BasicAttack, Maybe<Member>.Missing(), ResourceQuantity.None));

    [Test]
    public void Interpolated_AttackWithOwner_IsCorrect() 
        => AssertMatchesIgnoreStyling("8", ForEffect(BasicAttack, Owner, ResourceQuantity.None));

    [Test]
    public void Interpolated_ResourceIcon_IsCorrect()
        => AssertContainsSprite(Description("Flames", BasicAttack, Owner));
    
    [Test]
    public void Interpolated_Duration_IsCorrect()
        => AssertMatchesIgnoreStyling("Deal 3 for 2 turns.", 
            Description("Deal 3 {D[0]}", new EffectData {NumberOfTurns = new IntReference(2)}, Owner));

    [Test]
    public void Interpolated_XCost_WithOwner_IsCorrect()
        => AssertMatchesIgnoreStyling("6", Description("{X}", new EffectData(), Owner, new ResourceQuantity { Amount = 6 }));
    
    [Test]
    public void Interpolated_XCost_WithoutOwner_IsCorrect()
        => AssertMatchesIgnoreStyling("X", Description("{X}", new EffectData(), Maybe<Member>.Missing()));
    
    [Test]
    public void Interpolated_XCost_LockedIn_IsCorrect()
        => AssertMatchesIgnoreStyling("3", Description("{X}", new EffectData(), Maybe<Member>.Missing(), new ResourceQuantity { Amount = 3 }));
    
    [Test]
    public void Interpolated_Injury_IsCorrect()
        => AssertMatchesIgnoreStyling("Injury: -8 Attack", Description("Injury: {E[0]}", new EffectData
        {
            EffectType = EffectType.ApplyAdditiveStatInjury,
            EffectScope = new StringReference("Attack"),
            FloatAmount = new FloatReference(-8)
        }, Owner));

    [Test]
    public void Interpolated_ReactionEffect_IsCorrect()
        => AssertMatchesIgnoreStyling("On attacked, deal 8 damage",
            ReactionDescription("On attacked, deal {RE[0]} damage", 
                new EffectData
                {
                    EffectType = EffectType.Attack, 
                    BaseAmount = new IntReference(8)
                }
            , Owner));

    [Test]
    public void Interpolated_RawDamageFormula_IsCorrect()
        => AssertMatchesIgnoreStyling("Deal 8 raw damage",
            Description("Deal {E[0]} raw damage", new EffectData {EffectType = EffectType.DealRawDamageFormula, Formula = "Shield * 2"},
                OwnerWith(m => m.State.AdjustShield(4))));
    
    [Test]
    public void Interpolated_Auto_GivesVulnerableNextTurn_IsCorrect()
        => AssertMatchesIgnoreStyling("Next turn, gives Vulnerable for 1 turn.",
            Description("{Auto}", 
                new EffectData {EffectType = EffectType.ApplyVulnerable, TurnDelay = 1, NumberOfTurns = new IntReference(1)},
                Owner));

    private string Description(string s, EffectData e, Maybe<Member> owner)
        => Description(s, e, owner, ResourceQuantity.None);
    private string Description(string s, EffectData e, Maybe<Member> owner, ResourceQuantity xCost) 
        => InterpolatedCardDescriptions.InterpolatedDescription(s, new[] {e}, new EffectData[0], owner, xCost, Maybe<CardTypeData>.Missing());
    private string ReactionDescription(string s, EffectData re, Maybe<Member> owner)
        => ReactionDescription(s, re, owner, ResourceQuantity.None);
    private string ReactionDescription(string s, EffectData re, Maybe<Member> owner, ResourceQuantity xCost) 
        => InterpolatedCardDescriptions.InterpolatedDescription(s, new EffectData[0], new [] {re}, owner, xCost, Maybe<CardTypeData>.Missing());
    private string ForEffect(EffectData e, Maybe<Member> owner, ResourceQuantity xCost) => InterpolatedCardDescriptions.EffectDescription(e, owner, xCost);

    private void AssertContainsSprite(string actual) => Assert.IsTrue(actual.Contains("<sprite index="));
    
    private void AssertMatchesIgnoreStyling(string expected, string actual)
    {
        var unstyledActual = actual
            .Replace("<b>", "")
            .Replace("</b>", "")
            .Replace(" <sprite index=", "");
        Enumerable.Range(0, 32)
            .ForEach(i => unstyledActual = unstyledActual.Replace($"{i}>", ""));
        Assert.AreEqual(expected, unstyledActual);
    }
    
    private Member OwnerWith(Action<Member> update)
    {
        var m = new Member(0, "", "", TeamType.Enemies,
            new StatAddends()
                .With(StatType.Damagability, 1)
                .With(StatType.Attack, 8)
                .With(StatType.Toughness, 8)
                .With(StatType.MaxShield, 16)
                .With(new InMemoryResourceType("Ammo") {MaxAmount = 6, StartingAmount = 6}),
            BattleRole.Unknown);
        update(m);
        return m;
    }

}
