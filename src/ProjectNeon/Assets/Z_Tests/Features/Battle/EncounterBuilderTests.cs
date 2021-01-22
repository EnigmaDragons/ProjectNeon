using System.Linq;
using NUnit.Framework;

public class EncounterBuilderTests
{
    [Test, Ignore("Doesn't work with new power level system.")]
    public void EncounterBuilder_Generate_ReturnsAnyEnemies()
    {
        var enemy = TestableObjectFactory.Create<Enemy>();
        var encounterBuilder = TestableObjectFactory.Create<EncounterBuilder>();
        encounterBuilder.Init(enemy.AsArray());

        var encounter = encounterBuilder.Generate(10);

        Assert.IsTrue(encounter.Any());
    }
}
