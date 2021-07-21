
using NUnit.Framework;

public class RngTests
{
    [Test]
    public void Rng_ArrayElement_NoIndexOutOfRange()
    {
        var arr = new[] {1};

        for (var i = 0; i < 1000; i++)
            arr.Random();
    }
}