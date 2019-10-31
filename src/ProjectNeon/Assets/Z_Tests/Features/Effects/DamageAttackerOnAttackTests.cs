using NUnit.Framework;
using UnityEngine;

public sealed class DamageAttackerOnAttackTests
{
    private EffectData DamageAttackerOnAttack(float amount) => 
        new EffectData { 
            EffectType = EffectType.DamageAttackerOnAttack, 
            FloatAmount = new FloatReference(amount) 
        };

    [Test]
    public void DamageOnAttacker_ApplyEffect_AttackerIsDamagedOnAttack()
    {
        Member paladin = TestMembers.Create(s => s.With(StatType.Attack, 1f));
        Member ally = TestMembers.Any();
        Member attacker = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        AllEffects.Apply(DamageAttackerOnAttack(1), paladin, new MemberAsTarget(ally));
        BattleEvent.Publish(new Attack(attacker, ally));

        Assert.AreEqual(9, attacker.State[TemporalStatType.HP]);
    }

    [Test]
    public void DamageOnAttacker_ApplyEffect_AttackerIsNotDamagedOnAttackingOther()
    {
        Member paladin = TestMembers.Create(s => s.With(StatType.Attack, 1f));
        Member ally = TestMembers.Any();
        Member attacker = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        AllEffects.Apply(DamageAttackerOnAttack(1), paladin, new MemberAsTarget(ally));
        BattleEvent.Publish(new Attack(attacker, paladin));

        Assert.AreEqual(10, attacker.State[TemporalStatType.HP]);
    }
}
