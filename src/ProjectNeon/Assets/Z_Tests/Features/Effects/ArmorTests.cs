
using NUnit.Framework;

public class ArmorTests
{
    [Test]
    public void With0Armor_ReduceArmor_ArmorIs0()
    {
        var member = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 5));
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustStatAdditively,
            EffectScope = new StringReference(StatType.Armor.ToString()),
            FloatAmount = new FloatReference(-1)
        }, attacker, member );

        Assert.AreEqual(0, member.Armor());
    }
}
