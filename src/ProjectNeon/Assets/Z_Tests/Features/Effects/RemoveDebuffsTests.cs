using System.Collections.Generic;
using NUnit.Framework;

public class RemoveDebuffsTests
{
    [Test]
    public void RemoveDebuffs_ApplyEffect_RemovesAdditiveEffect()
    {
        var removeDebuffs = new EffectData { EffectType = EffectType.RemoveDebuffs };
        var target = TestMembers.Create(s => s.With(StatType.Attack, 10));
        target.State.ApplyTemporaryAdditive(new AdjustedStats(new StatAddends().With(StatType.Attack, -5), 
            TemporalStateMetadata.DebuffForDuration(5)));

        TestEffects.Apply(removeDebuffs, TestMembers.Any(), target);

        Assert.AreEqual(10, target.State[StatType.Attack]);
    }

    [Test]
    public void RemoveDebuffs_ApplyEffect_RemovesMultiplicitiveEffect()
    {
        var removeDebuffs = new EffectData { EffectType = EffectType.RemoveDebuffs };
        var target = TestMembers.Create(s => s.With(StatType.Attack, 10));
        target.State.ApplyTemporaryMultiplier(new AdjustedStats(new StatAddends().With(StatType.Attack, -5),
            TemporalStateMetadata.DebuffForDuration(5)));

        TestEffects.Apply(removeDebuffs, TestMembers.Any(), target);

        Assert.AreEqual(10, target.State[StatType.Attack]);
    }

    [Test]
    public void RemoveDebuffs_ApplyEffect_RemovesReactiveState()
    {
        var removeDebuffs = new EffectData { EffectType = EffectType.RemoveDebuffs };
        var target = TestMembers.Create(s => s);
        target.State.AddReactiveState(new ReactOnAttacked(true, 2, 2, new StatusDetail(StatusTag.CounterAttack), ReactiveTriggerScope.All, new Dictionary<int, Member>(), 1, target, TestCards.AnyReaction()));

        TestEffects.Apply(removeDebuffs, TestMembers.Any(), target);

        Assert.False(target.State.HasStatus(StatusTag.CounterAttack));
    }

    [Test]
    public void RemoveDebuffs_ApplyEffect_DoesNotRemoveBuff()
    {
        var removeDebuffs = new EffectData { EffectType = EffectType.RemoveDebuffs };
        var target = TestMembers.Create(s => s);
        target.State.AddReactiveState(new ReactOnAttacked(false, 2, 2, 
            new StatusDetail(StatusTag.CounterAttack), 
            ReactiveTriggerScope.All, 
            new Dictionary<int, Member> { {target.Id, target}}, target.Id,  target, TestCards.AnyReaction()));

        TestEffects.Apply(removeDebuffs, TestMembers.Any(), target);

        Assert.True(target.State.HasStatus(StatusTag.CounterAttack));
    }
}
