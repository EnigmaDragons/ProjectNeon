using NUnit.Framework;
using UnityEngine;

public sealed class DamageAttackerOnAttackTests
{
    private EffectData DamageAttackerOnAttack(float amount) => 
        new EffectData { 
            EffectType = EffectType.ShieldAttackedOnAttack, 
            FloatAmount = new FloatReference(amount) 
        };

    [Test]
    public void DamageOnAttacker_ApplyEffect_AttackerIsDamagedOnAttack()
    {
        Member paladin = TestMembers.With(StatType.Toughness, 5);
        Member ally = TestMembers.With(StatType.Toughness, 10);
        Member attacker = TestMembers.Any();

        AllEffects.Apply(DamageAttackerOnAttack(1), paladin, new MemberAsTarget(ally));

        BattleEvent.Publish(new Attack(attacker, new MemberAsTarget(ally)));

        Assert.AreEqual(5, ally.State[TemporalStatType.Shield]);
    }

    [Test]
    public void DamageOnAttacker_ApplyEffect_AttackerIsNotDamagedIfNotAttacks()
    {
        Member paladin = TestMembers.Any();
        Member ally = TestMembers.Any();
        Member attacker = TestMembers.Any();

        AllEffects.Apply(DamageAttackerOnAttack(1), paladin, new MemberAsTarget(ally));

        Assert.AreEqual(attacker.State.MaxHP(), attacker.State[TemporalStatType.HP]);
    }
}
