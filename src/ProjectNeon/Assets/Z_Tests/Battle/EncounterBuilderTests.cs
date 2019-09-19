using System.Linq;
using NUnit.Framework;

public class EncounterBuilderTests
{
    [Test]
    public void EncounterBuilder_Generate_ReturnsAnyEnemies()
    {
        var enemy = TestableObjectFactory.Create<Enemy>();
        var encounterBuilder = TestableObjectFactory.Create<EncounterBuilder>();
        encounterBuilder.Init(enemy.AsArray(), 10);

        var encounter = encounterBuilder.Generate();

        Assert.IsTrue(encounter.Any());
    }
}

