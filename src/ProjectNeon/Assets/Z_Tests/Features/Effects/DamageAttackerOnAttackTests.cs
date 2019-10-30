using NUnit.Framework;
using UnityEngine;

public sealed class DamageAttackerOnAttackTests
{
    private EffectData DamageAttackerOnAttack(float amount) => 
        new EffectData { 
            EffectType = EffectType.PhysicalDamage, 
            FloatAmount = new FloatReference(amount) 
        };

    [Test]
    public void DamageOnAttacker_ApplyEffect_AttackerIsDamagedOnAttack()
    {
        Member paladin = TestMembers.Create(s => s.With(StatType.Attack, 1f).With(StatType.Damagability, 1f));
        Member ally = TestMembers.Any();
        Member attacker = TestMembers.Create(s => s.With(StatType.Attack, 1).With(StatType.Damagability, 1f).With(StatType.MaxHP, 10));

        AllEffects.Apply(DamageAttackerOnAttack(1), paladin, new MemberAsTarget(ally));
        BattleEvent.Publish(new Attack(attacker, new MemberAsTarget(ally)));

        Assert.AreEqual(8, attacker.State[TemporalStatType.HP]);
    }

    [Test]
    public void DamageOnAttacker_ApplyEffect_AttackerIsNotDamagedOnAttackingOther()
    {
        Member paladin = TestMembers.Any();
        Member ally = TestMembers.Any();
        Member attacker = TestMembers.Any();

        AllEffects.Apply(DamageAttackerOnAttack(1), paladin, new MemberAsTarget(ally));
        BattleEvent.Publish(new Attack(attacker, new MemberAsTarget(paladin)));

        Assert.AreEqual(attacker.State.MaxHP(), attacker.State[TemporalStatType.HP]);
    }
}
