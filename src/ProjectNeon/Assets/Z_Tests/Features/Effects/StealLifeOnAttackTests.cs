using NUnit.Framework;

public sealed class StealLifeOnAttackTests
{
    private EffectData StealLifeOnAttack(float amount) => new EffectData {
        EffectType = EffectType.StealLifeOnAttack,
        FloatAmount = new FloatReference(amount)
    }; 
    
    [Test]
    public void ArmorFlat_ApplyEffect_ArmorIsChangedCorrectly()
    {
        Member caster = TestMembers.Any();
        Member attacker = TestMembers.Create(
            s => s.With(StatType.Attack, 1f).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        attacker.State.TakeRawDamage(6);

        AllEffects.Apply(StealLifeOnAttack(1), caster, new MemberAsTarget(attacker));
        BattleEvent.Publish(new Attack(attacker, target));

        Assert.AreEqual(5, attacker.State[TemporalStatType.HP]);
    }
}
