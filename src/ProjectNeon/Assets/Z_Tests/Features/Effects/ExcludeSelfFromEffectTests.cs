using NUnit.Framework;

public class ExcludeSelfFromEffectTests
{
    [Test]
    public void ExcludeSelfFromEffect_ApplyEffect_ExcludeSelfAndApplyAlly()
    {
        var caster = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        var ally = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        caster.State.TakeRawDamage(1);
        ally.State.TakeRawDamage(1);

        new ExcludeSelfFromEffect(new Heal(5)).Apply(caster, new Multiple(new[] { caster , ally }));

        Assert.AreEqual(9, caster.State[TemporalStatType.HP], "Applied effect to self");
        Assert.AreEqual(10, ally.State[TemporalStatType.HP], "Did not applied effect to ally");
    }
}
