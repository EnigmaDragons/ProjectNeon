using NUnit.Framework;

public sealed class HealFlatTests
{
    [Test]
    public void HealFlat_ApplyEffect_DoesNotPassFullHealth()
    {
        var heal5 = new EffectData { EffectType = EffectType.HealFlat, FloatAmount = new FloatReference(5) };
        var target = TestMembers.With(StatType.MaxHP, 10);
        
        AllEffects.Apply(heal5, TestMembers.Any(), new MemberAsTarget(target));
        
        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void HealFlat_Take6DamageAndThenHeal5_HpIsCorrect()
    {
        var heal5 = new EffectData { EffectType = EffectType.HealFlat, FloatAmount = new FloatReference(5) };
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));
        
        target.State.TakeRawDamage(6);
        AllEffects.Apply(heal5, TestMembers.Any(), new MemberAsTarget(target));
        
        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
}
