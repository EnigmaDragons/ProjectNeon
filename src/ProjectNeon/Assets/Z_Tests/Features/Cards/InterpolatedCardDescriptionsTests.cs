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
        => Assert.AreEqual("1x ATK", ForEffect(BasicAttack, Maybe<Member>.Missing()));

    [Test]
    public void Interpolated_AttackWithOwner_IsCorrect() 
        => Assert.AreEqual("8", ForEffect(BasicAttack, Owner));

    [Test]
    public void Interpolated_Duration_IsCorrect()
        => AssertMatchesIgnoreStyling("Deal 3 for 2 Turns.", 
            Description("Deal 3 {D[0]}", new EffectData {NumberOfTurns = new IntReference(2)}, Owner));

    [Test]
    public void Interpolated_XCost_IsCorrect()
        => AssertMatchesIgnoreStyling("6", Description("{X}", new EffectData(), Owner));
    
    [Test]
    public void Interpolated_Injury_IsCorrect()
        => AssertMatchesIgnoreStyling("Injury: -8 Attack", Description("Injury: {E[0]}", new EffectData
        {
            EffectType = EffectType.ApplyAdditiveStatInjury,
            EffectScope = new StringReference("Attack"),
            FloatAmount = new FloatReference(-8)
        }, Owner));

    private string Description(string s, EffectData e, Maybe<Member> owner) => InterpolatedCardDescriptions.InterpolatedDescription(s, new[] {e}, owner);
    private string ForEffect(EffectData e, Maybe<Member> owner) => InterpolatedCardDescriptions.GenerateEffectDescription(e, owner);

    private void AssertMatchesIgnoreStyling(string expected, string actual)
    {
        var unstyledActual = actual.Replace("<b>", "").Replace("</b>", "");
        Assert.AreEqual(expected, unstyledActual);
    }
}
