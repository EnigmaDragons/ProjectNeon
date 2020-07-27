
using NUnit.Framework;
using UnityEngine;

public sealed class InterceptAttackTests
{
    [Test]
    public void InterceptAttack_ApplyEffect_AttackIsIntercepted()
    {
        Member paladin = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        Member ally = TestMembers.With(StatType.MaxHP, 10);
        Member attacker = TestMembers.With(StatType.Attack, 1);

        new InterceptAttack(1).Apply(paladin, new Single(ally));
        new Attack(5).Apply(attacker, new Single(ally));

        Assert.AreEqual(
            5,
            paladin.State[TemporalStatType.HP],
            "Interceptor didn't received the interception damage"
        );

        Assert.AreEqual(
            10,
            ally.State[TemporalStatType.HP],
            "Attack damaged ally"
        );
    }
}
