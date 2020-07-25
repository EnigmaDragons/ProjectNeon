using NUnit.Framework;


public sealed class ResourceFlat
{

    private EffectData data = new EffectData { 
        EffectType = EffectType.ResourceFlat, 
        FloatAmount = new FloatReference(5), 
        EffectScope = new StringReference(Ammo.Name) 
    } ;

    private static IResourceType Ammo = new InMemoryResourceType {Name = "Ammo", MaxAmount = 99, StartingAmount = 0};
    
    private Member performer = new Member(
        1,
        "Good Dummy One",
        "ElectroBlade",
        TeamType.Party,
        new StatAddends()
        {
            ResourceTypes = new IResourceType[] { Ammo }
        }
    );

    [Test]
    public void ResourceFlat_ApplyEffect()
    {
        AllEffects.Apply(data, performer, new Single(performer));
        Assert.AreEqual(
            5,
            performer.State[Ammo]
        );
    }
}
