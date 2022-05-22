using NUnit.Framework;

public class ResetStatToBase
{
    [Test]
    public void ResetStatToBase_WasReset()
    {
        var performer = TestMembers.Create("Missile Launcher", TeamType.Enemies, s => s);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.AdjustStatAdditivelyFormula,
            EffectScope = new StringReference(StatType.Armor.ToString()),
            Formula = "5",
            DurationFormula = "-1"
        }, performer, new Single(performer));
        Assert.AreEqual(5, performer.State[StatType.Armor]);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ResetStatToBase,
            EffectScope = new StringReference(StatType.Armor.ToString()),
        }, performer, new Single(performer));
        Assert.AreEqual(0, performer.State[StatType.Armor]);
    }
}
