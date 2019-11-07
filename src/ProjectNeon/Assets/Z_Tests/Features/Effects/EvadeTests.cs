
using NUnit.Framework;
using UnityEngine;

public sealed class EvadeTests
{

    [Test]
    public void Evade_ApplyEffect_AttackIsEvaded()
    {
        Member evader = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        Member attacker = TestMembers.With(StatType.Attack, 1);

        new Evade().Apply(evader, new MemberAsTarget(evader));

        new Attack(5).Apply(attacker, evader);

        Assert.AreEqual(
            10,
            evader.State[TemporalStatType.HP],
            "Target did not evaded attack"
        );
    }
}
