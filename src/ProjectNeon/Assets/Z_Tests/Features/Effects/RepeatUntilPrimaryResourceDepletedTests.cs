using NUnit.Framework;
using UnityEngine;
using System.Linq;

public sealed class RepeatUntilPrimaryResourceDepletedTest
{
    private EffectData DamageTarget(float amount) => new EffectData
    {
        EffectType = EffectType.PhysicalDamage,
        FloatAmount = new FloatReference(amount)
    };

    [Test]
    public void RepeatUntilPrimaryResourceDepleted_ApplyEffect_ApplyWhileHaveResource()
    {
        Effect twoTimer = new RepeatUntilPrimaryResourceDepleted(
            new CostPrimaryResourceEffect(
                AllEffects.Create(DamageTarget(1)),
                1
            ),
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
                        MaxAmount = 2
                    }
                ).ToArray();
                addend.With(StatType.Attack, 1);
                return addend;
            }
        );
        attacker.State.GainPrimaryResource(2);
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        Debug.Log("Let's go");

        twoTimer.Apply(attacker, new Single(target));

        Debug.Log("Done!");

        Assert.AreEqual(
            8,
            target.State[TemporalStatType.HP],
            "Effect not applied correct number of times."
        );

    }
}
