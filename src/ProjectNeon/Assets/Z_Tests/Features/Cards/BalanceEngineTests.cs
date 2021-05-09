using NUnit.Framework;

public class BalanceEngineTests
{
    [Test]
    public void BalanceEngine_BasicGain3Energy_IsBalanced() 
        => AssertIsBalanced(new InMemoryCard
        {
            Name = "Energize",
            Rarity = Rarity.Basic,
            Gain = new InMemoryResourceAmount(3, "Energy")
        });

    [Test]
    public void BalanceEngine_BasicScalingAttack_IsBalanced() 
        => AssertIsBalanced(new InMemoryCard
        {
            Name = "Slash",
            Rarity = Rarity.Basic,
            ActionSequences = new[] { CardActionSequence.Create(
                Scope.One, Group.Opponent,
                TestableObjectFactory.Create<CardActionsData>()
                    .Initialized(new CardActionV2(TestEffects.BasicAttack)), false), }
        });

    private void AssertIsBalanced(CardTypeData c)
        => AssertIsBalanced(BalanceEngine.Assess(c));
    
    private void AssertIsBalanced(BalanceAssessment b)
        => Assert.AreEqual(false, b.NeedsAdjustment, $"{b.CardName} is not balanced. Target Power {b.TargetPower}. Actual Power: {b.ActualPower}");
}
