using UnityEngine;
using System.Collections;
using NUnit.Framework;

public class ExcludeSelfFromEffectTests : MonoBehaviour
{

    [Test]
    public void ExcludeSelfFromEffect_ApplyEffect_ExcludeSelfAndApplyAlly()
    {
        Member caster = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        Member ally = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        caster.State.TakeRawDamage(1);
        ally.State.TakeRawDamage(1);

        new ExcludeSelfFromEffect(new Heal(5)).Apply(caster, new Multiple(new Member[]{ caster , ally}));

        Assert.AreEqual(9, caster.State[TemporalStatType.HP], "Applied effect to self");
        Assert.AreEqual(10, ally.State[TemporalStatType.HP], "Did not applied effect to ally");
    }
}
