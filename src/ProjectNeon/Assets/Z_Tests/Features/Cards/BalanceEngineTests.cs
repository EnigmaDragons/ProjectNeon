using NUnit.Framework;

public class BalanceEngineTests
{
    [Test]
    public void BalanceEngine_CommonGain3Energy_IsBalanced() 
        => AssertIsBalanced(new InMemoryCard
        {
            Name = "Energize",
            Rarity = Rarity.Common,
            Gain = new InMemoryResourceAmount(3, "Energy")
        });

    [Test]
    public void BalanceEngine_CommonScalingAttack_IsBalanced() 
        => AssertIsBalanced(new InMemoryCard
        {
            Name = "Slash",
            Rarity = Rarity.Common,
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
