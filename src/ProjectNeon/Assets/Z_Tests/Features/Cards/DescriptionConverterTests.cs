using NUnit.Framework;

public class DescriptionConverterTests
{
    [Test]
    public void NoSymbols_ConversionCorrect()
        => AssertConverted("Stealth", "Stealth");

    [Test]
    public void OneSymbol_ConversionCorrect() 
        => AssertConverted("Deal {E[0]}", "Deal {0}", "{E[0]}");

    [Test]
    public void TwoSymbols_ConversionCorrect()
        => AssertConverted("Deal {E[0]} {D[0]}", "Deal {0} {1}", "{E[0]}", "{D[0]}");

    private void AssertConverted(string descV1, string expectedDescV2, params string[] args)
    {
        var actualDescV2 = CardDescriptionV2.FromDescriptionV1(descV1);
        
        Assert.AreEqual(expectedDescV2, actualDescV2.text);
        CollectionAssert.AreEquivalent(args, actualDescV2.formatArgs);
    }
}
