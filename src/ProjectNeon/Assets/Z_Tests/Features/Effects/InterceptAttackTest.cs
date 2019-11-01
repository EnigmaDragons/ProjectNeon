
using NUnit.Framework;

public sealed class InterceptAttackTests
{
    [Test]
    public void InterceptAttack_ApplyEffect_AttackIsIntercepted()
    {
        Member paladin = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        Member ally = TestMembers.With(StatType.MaxHP, 10);
        Member attacker = TestMembers.With(StatType.Attack, 5);
        Attack attack = new Attack(attacker, ally, 5);

        AllEffects.Apply(new EffectData { EffectType = EffectType.InterceptAttack }, paladin, new MemberAsTarget(ally));
        BattleEvent.Publish(attack);

        Assert.AreEqual(
            paladin,
            attack.Target,
            "Paladin didn't intercepted attack"
        );
    }
}
