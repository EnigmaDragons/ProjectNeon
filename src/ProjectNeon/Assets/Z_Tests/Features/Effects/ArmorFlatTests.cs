using NUnit.Framework;


public sealed class ArmorFlatTests
{

    private EffectData data = new EffectData { EffectType = EffectType.ArmorFlat, FloatAmount = new FloatReference(1) } ;

    private Member performer = new Member(
        1,
        "Good Dummy One",
        "Paladin",
        TeamType.Party,
        new StatAddends()
    );

    private Member target = new Member(
        2,
        "Good Dummy Two",
        "Wooden Dummy",
        TeamType.Party,
        new StatAddends().With(StatType.Armor, 5)
    );

    [Test]
    public void ArmorFlat_ApplyEffect()
    {
        AllEffects.Apply(data, performer, new MemberAsTarget(target));
        Assert.AreEqual(
            6,
            target.State.Armor()
        );
    }
}
