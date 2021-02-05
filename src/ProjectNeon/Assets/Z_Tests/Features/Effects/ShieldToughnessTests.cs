using NUnit.Framework;

[TestFixture]
public sealed class ShieldToughnessTests
{
    [Test]
    public void ShieldToughness_ApplyEffect()
    {
        var shieldTarget = new EffectData { EffectType = EffectType.ShieldFormula, Formula = "1 * Toughness"};
        var performer = TestMembers.With(StatType.Toughness, 5);
        var target = TestMembers.Create(s => s.With(StatType.Toughness, 999).With(StatType.MaxShield, 10));
        
        TestEffects.Apply(shieldTarget, performer, new Single(target));
        Assert.AreEqual(5, target.State[TemporalStatType.Shield]);
    }
}
