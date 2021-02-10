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
                Scope.One, Group.Opponent, AvoidanceType.Evade, 
                TestableObjectFactory.Create<CardActionsData>()
                    .Initialized(new CardActionV2(TestEffects.BasicAttack)), false), }
        });
    
    [Test]
    public void BalanceEngine_UncommonScalingAttack_IsBalanced() 
        => AssertIsBalanced(new InMemoryCard
        {
            Name = "Slash",
            Rarity = Rarity.Uncommon,
            ActionSequences = new[] { CardActionSequence.Create(
                Scope.One, Group.Opponent, AvoidanceType.Evade, 
                TestableObjectFactory.Create<CardActionsData>()
                    .Initialized(new CardActionV2(new EffectData
                    {
                        EffectType = EffectType.Attack,
                        FloatAmount = new FloatReference(1.2f)
                    })), false), }
        });

    [Test]
    public void BalanceEngine_WithMultiplicativeStatDebuff_IsBalanced()
        => AssertIsBalanced(new InMemoryCard
        {
            Name = "Covering Fire",
            Rarity = Rarity.Uncommon,
            Cost = new InMemoryResourceAmount(2, "Ammo"),
            ActionSequences = new[] { CardActionSequence.Create(
                Scope.One, Group.Opponent, AvoidanceType.Evade, 
                TestableObjectFactory.Create<CardActionsData>()
                    .Initialized(new CardActionV2(new EffectData
                    {
                        EffectType = EffectType.Attack,
                        FloatAmount = new FloatReference(1.2f)
                    }), new CardActionV2(new EffectData
                    {
                        EffectType = EffectType.AdjustStatMultiplicatively,
                        FloatAmount = new FloatReference(0.7f)
                    })), false), }
        });

    private void AssertIsBalanced(CardTypeData c)
        => AssertIsBalanced(BalanceEngine.Assess(c));
    
    private void AssertIsBalanced(BalanceAssessment b)
        => Assert.AreEqual(false, b.NeedsAdjustment, $"{b.CardName} is not balanced. Target Power {b.TargetPower}. Actual Power: {b.ActualPower}");
}
