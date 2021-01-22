using NUnit.Framework;

public class AdjustStatsAdditivelyTests
{
    [Test]
    public void AdjustStatAdditively_ApplyEffect_CharactersStatsAdjusted()
    {
        var adjustment = new EffectData
        {
            EffectType = EffectType.AdjustStatAdditivelyFormula, 
            EffectScope = new StringReference("Attack"), 
            NumberOfTurns = new IntReference(-1),
            Formula = "1"
        };
        var target = TestMembers.Create(s => s.With(StatType.Attack, 10));

        TestEffects.Apply(adjustment, TestMembers.Any(), target);

        Assert.AreEqual(11, target.State[StatType.Attack]);
    }

    [Test]
    public void AdjustStatAdditively_ApplyMaxHPEffect_CharactersStatsAdjusted()
    {
        var adjustment = new EffectData
        {
            EffectType = EffectType.AdjustStatAdditivelyFormula, 
            EffectScope = new StringReference("MaxHP"), 
            NumberOfTurns = new IntReference(-1),
            Formula = "1"
        };
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));

        TestEffects.Apply(adjustment, TestMembers.Any(), target);

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
        Assert.AreEqual(11, target.State[StatType.MaxHP]);
    }
}
