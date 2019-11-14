using NUnit.Framework;
using UnityEngine;
using System.Linq;

public sealed class CostPrimaryResourcesEffectTest
{
    private EffectData DamageTarget(float amount) => new EffectData
    {
        EffectType = EffectType.PhysicalDamage,
        FloatAmount = new FloatReference(amount)
    };

    [Test]
    public void CostPrimaryResourcesEffect_ApplyEffect_ApplyWhenHaveResource()
    {
        Effect oneTimer = new CostPrimaryResourceEffect(
            AllEffects.Create(DamageTarget(1)),
            1
        );
        Member attacker = TestMembers.Create(
            s => {
                StatAddends addend = new StatAddends();
                addend.ResourceTypes = addend.ResourceTypes.Concat(
                    new InMemoryResourceType
                    {
                        Name = "Resource",
                        StartingAmount = 0,
                        MaxAmount = 1
                    }
                ).ToArray();
                addend.With(StatType.Attack, 1);
                return addend;
            }
        );
        attacker.State.GainPrimaryResource(1);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        oneTimer.Apply(attacker, new Single(target));

        Assert.AreEqual(
            9,
            target.State[TemporalStatType.HP],
            "Effect not applied."
        );

        oneTimer.Apply(attacker, new Single(target));
        Assert.AreEqual(
            9,
            target.State[TemporalStatType.HP],
            "Effect applied when not having resources."
        );
    }
}
