using NUnit.Framework;

public class BalanceEngineTests
{
    [Test]
    public void BalanceEngine_CommonGain3Energy_IsBalanced()
    {
        var c = new InMemoryCard
        {
            Name = "Energize",
            Rarity = Rarity.Common,
            Gain = new InMemoryResourceAmount(3, "Energy")
        };

        var assessment = BalanceEngine.Assess(c);
        
        Assert.AreEqual(false, assessment.NeedsAdjustment);
        Assert.AreEqual("Energize", assessment.CardName);
    }
}
