using NUnit.Framework;

public sealed class ArmorFlatTests
{
    private EffectData ChangeArmorBy(float amount) => new EffectData { EffectType = EffectType.ArmorFlat, FloatAmount = new FloatReference(amount) }; 
    
    [Test]
    public void ArmorFlat_ApplyEffect_ArmorIsChangedCorrectly()
    {
        var addArmorEffect = ChangeArmorBy(1);
        var target = TestMembers.With(StatType.Armor, 5);
        
        AllEffects.Apply(addArmorEffect, TestMembers.Any(), new MemberAsTarget(target));
        
        Assert.AreEqual(6, target.State.Armor());
    }
}
