using NUnit.Framework;

public sealed class NotFeedOnEffectTests
{

    [Test]
    public void NotFeedOnEffect_ApplyEffect_EffectIsAppliedIfNotFeedOn()
    {
        Member attacker = TestMembers.Create(
            s => s.With(StatType.Attack, 1f).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        new NotFeedOnEffect(new Attack(1), "LUNAR").Apply(attacker, new MemberAsTarget(target));

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void NotFeedOnEffect_ApplyEffect_EffectIsNotAppliedIfFeedOn()
    {
        Member attacker = TestMembers.Create(
            s => s.With(StatType.Attack, 1f).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        attacker.State.ChangeStatus("FEED", "LUNAR");
        new NotFeedOnEffect(new Attack(1), "LUNAR").Apply(attacker, new MemberAsTarget(target));

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
    }

    [Test]
    public void NotFeedOnEffect_ApplyEffect_EffectIsNotAppliedIfFeedOnAnother()
    {
        Member attacker = TestMembers.Create(
            s => s.With(StatType.Attack, 1f).With(StatType.MaxHP, 10).With(StatType.Damagability, 1f)
        );
        Member target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Damagability, 1f));

        attacker.State.ChangeStatus("FEED", "SOLAR");
        new NotFeedOnEffect(new Attack(1), "LUNAR").Apply(attacker, new MemberAsTarget(target));

        Assert.AreEqual(9, target.State[TemporalStatType.HP]);
    }
}
