using NUnit.Framework;

public class OnEvadedTests
{
    [Test, Ignore("Doesn't use new async Reaction resolution.")]
    public void OnEvaded_ApplyTwice_OnlyGeneratesOneReaction()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1));
        target.State.AdjustEvade(1);

        var reactionCardType = TestCards.Reaction(
            ReactiveMember.Possessor,
            ReactiveTargetScope.Self,
            new EffectData { EffectType = EffectType.AdjustStatAdditivelyFormula, Formula = "1", EffectScope = new StringReference("Armor"), NumberOfTurns = new IntReference(-1) });

        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnEvaded,
            NumberOfTurns = new IntReference(3),
            ReactionSequence = reactionCardType
        }, new EffectContext(target, new Single(target), Maybe<Card>.Missing()));

        ReactiveTestUtilities.ApplyEffectAndReactions(new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1),
            EffectScope = new StringReference(ReactiveTargetScope.Self.ToString())
        }, attacker, target);

        Assert.AreEqual(10, target.State[TemporalStatType.HP]);
        Assert.AreEqual(1, target.State.Armor());
    }
}
