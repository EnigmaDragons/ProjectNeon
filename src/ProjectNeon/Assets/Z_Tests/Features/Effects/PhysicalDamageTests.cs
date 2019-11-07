using NUnit.Framework;


public sealed class PhysicalDamageTests
{

    private EffectData data = new EffectData { EffectType = EffectType.PhysicalDamage, FloatAmount = new FloatReference(2) } ;

    private Member performer = new Member(
        1,
        "Good Dummy One",
        "ElectroBlade",
        TeamType.Party,
        new StatAddends().With(StatType.Attack, 1).With(StatType.Damagability, 1f)
    );

    private Member target;

    [Test]
    public void PhysicalDamageUnarmoredTarget_ApplyEffect()
    {
        target = new Member(
            2,
            "Target Dummy",
            "Soft Dummy",
            TeamType.Enemies,
            new StatAddends().With(StatType.Armor, 0).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );

        AllEffects.Apply(data, performer, new Single(target));
        Assert.AreEqual(
            8,
            target.State[TemporalStatType.HP]
        );
    }
}
