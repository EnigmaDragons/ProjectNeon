using NUnit.Framework;


public sealed class HealFlatTests
{

    private EffectData data = new EffectData { EffectType = EffectType.HealFlat, FloatAmount = new FloatReference(5) } ;

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
        new StatAddends().With(StatType.MaxHP, 10).With(StatType.Damagability, 1)
    );

    [Test]
    public void HealFlat_ApplyEffect_DoesNotPassFullHealth()
    {
        AllEffects.Apply(data, performer, new MemberAsTarget(target));
        Assert.AreEqual(
            10,
            target.State[TemporalStatType.HP]
        );
    }

    [Test]
    public void HealFlat_ApplyEffect()
    {
        target.State.TakeRawDamage(6);
        AllEffects.Apply(data, performer, new MemberAsTarget(target));
        Assert.AreEqual(
            9,
            target.State[TemporalStatType.HP]
        );
    }
}
