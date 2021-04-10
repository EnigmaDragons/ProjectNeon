using NUnit.Framework;

[TestFixture]
public class AegisTests
{
    [TestCase(TemporalStatType.Disabled)]
    [TestCase(TemporalStatType.Blind)]
    [TestCase(TemporalStatType.CardStun)]
    [TestCase(TemporalStatType.Inhibit)]
    [TestCase(TemporalStatType.Confused)]
    public void Aegis_NegativeCounters_PreventsAndConsumesAegisCounter(TemporalStatType statType)
    {
        var defender = DefenderWithAegis();
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(AdjustCounterEffect(statType), attacker, defender);
        
        Assert.AreEqual(0, defender.State[statType]);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }

    [Test]
    public void Aegis_DamageOverTime_PreventsAndConsumesAegisCounter()
    {
        var defender = DefenderWithAegis();
        var attacker = TestMembers.Any();
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.DamageOverTimeFormula, 
            Formula = "2", 
            NumberOfTurns = new IntReference(3)
        }, attacker, defender);
        
        Assert.AreEqual(0, defender.State.StatusesOfType(StatusTag.DamageOverTime).Length);
        Assert.AreEqual(0, defender.State[TemporalStatType.Aegis]);
    }

    private EffectData AdjustCounterEffect(TemporalStatType statType)
        => new EffectData
            {
                EffectType = EffectType.AdjustCounterFormula,
                EffectScope = new StringReference(statType.ToString()),
                Formula = "1"
            };

    private Member DefenderWithAegis(int numOfAegis = 1)
    {
        var defender = TestMembers.Any();
        defender.Apply(m => m.Adjust(TemporalStatType.Aegis, numOfAegis));
        return defender;
    }
}