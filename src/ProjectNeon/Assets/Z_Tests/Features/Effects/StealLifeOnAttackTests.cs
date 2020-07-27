using NUnit.Framework;

public sealed class StealLifeOnAttackTests
{
    private EffectData StealLifeOnAttack() => new EffectData {
        EffectType = EffectType.StealLifeNextAttack,
        FloatAmount = new FloatReference(1)
    }; 
    
    [Test]
    public void StealLifeOnAttack_ApplyEffect_LifeIsStolenCorrectly()
    {
        Member caster = TestMembers.Any();
        Member attacker = TestMembers.Create(
            s => s.With(StatType.Attack, 1f).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        attacker.State.TakeRawDamage(6);

        AllEffects.Apply(StealLifeOnAttack(), caster, new Single(attacker));
        new Attack(5).Apply(attacker, new Single(target));

        Assert.AreEqual(9, attacker.State[TemporalStatType.HP]);
    }
}
