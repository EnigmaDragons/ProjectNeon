
using NUnit.Framework;

[TestFixture]
public class HeroTests
{
    [Test]
    public void HeroEquipment_WithResourceModificationToPrimaryResource_IsCorrect()
    {
        var hero = new Hero(new InMemoryHeroCharacter 
            {
                Stats = new StatAddends()
                    .With(new InMemoryResourceType("Ammo")
                    {
                        StartingAmount = 4, MaxAmount = 4
                    })
            }, 
            new RuntimeDeck());

        var equipment = new InMemoryEquipment()
        {
            ResourceModifiers = ((IResourceType)new InMemoryResourceType("PrimaryResource") {StartingAmount = -1}).AsArray()
        };
        
        hero.Equip(equipment);
        
        Assert.AreEqual(1, hero.Stats.ResourceTypes.Length);
        Assert.AreEqual(3, hero.Stats.ResourceTypes[0].StartingAmount);
    }
}