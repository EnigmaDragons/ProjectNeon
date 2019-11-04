using NUnit.Framework;


public sealed class ResourceFlat
{

    private EffectData data = new EffectData { 
        EffectType = EffectType.ResourceFlat, 
        FloatAmount = new FloatReference(5), 
        EffectScope = new StringReference("Ammo") 
    } ;

    private Member performer = new Member(
        1,
        "Good Dummy One",
        "ElectroBlade",
        TeamType.Party,
        new StatAddends()
    );

    [Test]
    public void ResourceFlat_ApplyEffect()
    {
        AllEffects.Apply(data, performer, new MemberAsTarget(performer));
        Assert.AreEqual(
            5,
            performer.State.ResourceTypes[0].MaxAmount
        );
    }
}
