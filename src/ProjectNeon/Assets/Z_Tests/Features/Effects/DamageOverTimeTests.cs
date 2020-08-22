using NUnit.Framework;

public class DamageOverTimeTests
{
    [Test]
    public void DamageOverTime_Apply_DamageIsDealtCorrectlyOverTime()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        new DamageOverTime(new EffectData { FloatAmount = new FloatReference(2), NumberOfTurns = new IntReference(2) }).Apply(target, new Single(target));

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
        target.State.OnTurnEnd();
        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
        target.State.OnTurnStart();
        Assert.AreEqual(8, target.State[TemporalStatType.HP]);
        target.State.OnTurnEnd();
        target.State.OnTurnStart();
        Assert.AreEqual(6, target.State[TemporalStatType.HP]);
        target.State.OnTurnEnd();
        target.State.OnTurnStart();
        Assert.AreEqual(6, target.State[TemporalStatType.HP]);
    }
}