using NUnit.Framework;

public sealed class EffectCreationTests
{
    [Test]
    public void Effect_NoEffect_CanCreate()
    {
        Assert.IsNotNull(AllEffects.Create(new EffectData { EffectType = EffectType.Nothing }));
    }

    [Test]
    public void Effect_Heal_CanCreate()
    {
        Assert.IsNotNull(AllEffects.Create(new EffectData { EffectType = EffectType.HealFlat }));
    }
    
    [Test]
    public void Effect_DealPhysicalDamage_CanCreate()
    {
        Assert.IsNotNull(AllEffects.Create(new EffectData { EffectType = EffectType.PhysicalDamage }));
    }

    [Test]
    public void Effect_BuffAttackFlat_CanCreate()
    {
        Assert.IsNotNull(AllEffects.Create(new EffectData { EffectType = EffectType.BuffAttackFlat }));
    }
}
