using NUnit.Framework;

public class DamageOverTimeTests
{
    [Test]
    public void DamageOverTime_Apply_DamageIsDealtCorrectlyOverTime()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        TestEffects.Apply(new EffectData { 
            EffectType = EffectType.DamageOverTime, 
            FloatAmount = new FloatReference(2), 
            NumberOfTurns = new IntReference(2) 
        }, target, target);

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
        target.State.GetTurnEndEffects();
        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
        target.State.GetTurnStartEffects();
        Assert.AreEqual(8, target.State[TemporalStatType.HP]);
        target.State.GetTurnEndEffects();
        target.State.GetTurnStartEffects();
        Assert.AreEqual(6, target.State[TemporalStatType.HP]);
        target.State.GetTurnEndEffects();
        target.State.GetTurnStartEffects();
        Assert.AreEqual(6, target.State[TemporalStatType.HP]);
    }
}
