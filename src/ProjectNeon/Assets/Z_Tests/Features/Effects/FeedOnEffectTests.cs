using NUnit.Framework;

public sealed class FeedOnEffectTests
{
    
    [Test]
    public void FeedOnEffect_ApplyEffect_EffectIsAppliedCorrectlyIfFeedOn()
    {
        Member attacker = TestMembers.Create(
            s => s.With(StatType.Attack, 1f).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        attacker.State.FeedOn("Lunar");
        new FeedOnEffect(new Attack(1), "Lunar").Apply(attacker, new MemberAsTarget(target));

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void FeedOnEffect_ApplyEffect_EffectNotAppliedCorrectlyIfFeedOff()
    {
        Member attacker = TestMembers.Create(
            s => s.With(StatType.Attack, 1f).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new FeedOnEffect(new Attack(1), "Lunar").Apply(attacker, new MemberAsTarget(target));

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
    }
}
