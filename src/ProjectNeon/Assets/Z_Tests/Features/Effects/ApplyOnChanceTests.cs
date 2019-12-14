using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class ApplyOnChanceTests
{

    [Test]
    public void ApplyOnChanceTests_ApplyWhen1()
    {
        Member source = TestMembers.Any();
        Member target = TestMembers.Create(
            s => s.With(StatType.MaxHP, 10)
                .With(StatType.Damagability, 1f)
                .With(StatType.Toughness, 1f)
        );

        new ApplyOnChance(
            new SpellFlatDamageEffect(1), 1
        ).Apply(source, new Single(target));

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void ApplyOnChanceTests_DoesNotApplyWhen0()
    {
        Member source = TestMembers.Any();
        Member target = TestMembers.Create(
            s => s.With(StatType.MaxHP, 10)
                .With(StatType.Damagability, 1f)
                .With(StatType.Toughness, 1f)
        );

        new ApplyOnChance(
            new SpellFlatDamageEffect(1), 0
        ).Apply(source, new Single(target));

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
    }
}