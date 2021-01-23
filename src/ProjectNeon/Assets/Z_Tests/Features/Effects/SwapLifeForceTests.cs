using NUnit.Framework;

public class SwapLifeForceTests
{
    [Test]
    public void SwapLifeForce_Apply_LifeForceSwapped()
    {
        var source = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        source.State.TakeRawDamage(6);
        target.State.TakeRawDamage(4);

        new SwapLifeForce().Apply(source, target, Maybe<Card>.Missing());

        Assert.AreEqual(6, source.State[TemporalStatType.HP]);
        Assert.AreEqual(4, target.State[TemporalStatType.HP]);
    }
    
    [Test]
    public void SwapLifeForce_Apply_DoesNotExceedMaximumLife()
    {
        var source = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 20));
        source.State.TakeRawDamage(6);

        new SwapLifeForce().Apply(source, target, Maybe<Card>.Missing());

        Assert.AreEqual(10, source.State[TemporalStatType.HP]);
        Assert.AreEqual(4, target.State[TemporalStatType.HP]);
    }
}