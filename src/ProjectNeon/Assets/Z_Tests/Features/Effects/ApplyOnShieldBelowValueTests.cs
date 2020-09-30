using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class ApplyOnShieldBelowValueTests
{

    [Test]
    public void ApplyOnShieldBelowValue_ApplyEffectWhenShieldIsBelowValue()
    {
        Member source = TestMembers.Any();
        Member target = TestMembers.Create(
            s => s.With(StatType.MaxHP, 10)
                .With(StatType.Damagability, 1f)
                .With(StatType.Toughness, 1f)
        );

        new ApplyOnShieldBelowValue(
            new SpellFlatDamageEffect(1), 1
        ).Apply(source, new Single(target));

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void ApplyOnShieldBelowValue_DoesNotApplyEffectWhenShieldIsNotBelowValue()
    {
        Member source = TestMembers.Any();
        Member target = TestMembers.Create(
            s => s.With(StatType.MaxHP, 10)
                .With(StatType.Damagability, 1f)
                .With(StatType.Toughness, 1f)
        );

        target.State.AdjustShield(1);

        new ApplyOnShieldBelowValue(
            new SpellFlatDamageEffect(1), 1
        ).Apply(source, new Single(target));

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
    }
}