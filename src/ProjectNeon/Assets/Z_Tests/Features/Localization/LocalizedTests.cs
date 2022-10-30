using NUnit.Framework;

public class LocalizedTests
{
    [Test]
    public void ToI2Format_BoldTags_Correct()
    {
        var input = "Deal <b>Max Damage</b>";
        var output = input.ToI2Format();
        
        Assert.AreEqual("Deal {[B]}Max Damage{[/B]}", output);
        Assert.AreEqual(input, output.FromI2Format());
    }

    [Test]
    public void ToI2Format_StringFormatArgs_Correct()
    {
        var input = "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}";
        var output = input.ToI2Format();
        
        Assert.AreEqual("{[0]} {[1]} {[2]} {[3]} {[4]} {[5]} {[6]} {[7]} {[8]} {[9]}", output);
        Assert.AreEqual(input, output.FromI2Format());
    }
}
