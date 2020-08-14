using System.Linq;
using NUnit.Framework;

public sealed class ReactiveTriggerTests
{
    [Test]
    public void ReactiveTrigger_OnAttacked_GainedOneArmor()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1));

        var reactionCardType = TestCards.Reaction(
            ReactiveMember.Possessor, 
            ReactiveTargetScope.Self, 
            new EffectData { EffectType = EffectType.ArmorFlat, FloatAmount = new FloatReference(1) });

        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            ReactionSequence = reactionCardType
        }, target, target);
        
        ApplyEffectAndReactions(new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1),
            EffectScope = new StringReference(ReactiveTargetScope.Self.ToString())
        }, attacker, target);
        
        Assert.AreEqual(1, target.State.Armor());
    }

    [Test]
    public void ReactiveTrigger_OnAttacked_AttackerHitForOneDamage()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Attack, 1));
        var attacker = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Attack, 1));

        var reactionCardType = TestCards.Reaction(
            ReactiveMember.Originator, 
            ReactiveTargetScope.Attacker, 
            new EffectData { EffectType = EffectType.Attack, FloatAmount = new FloatReference(1) });

        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            ReactionSequence = reactionCardType
        }, target, target);
        
        ApplyEffectAndReactions(new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1),
            EffectScope = new StringReference(ReactiveTargetScope.Self.ToString())
        }, attacker, target);
        
        Assert.AreEqual(9, attacker.CurrentHp());
    }

    private void ApplyEffectAndReactions(EffectData e, Member source, Member target)
    {
        var battleSnapshotBefore = new BattleStateSnapshot(target.GetSnapshot());
        AllEffects.Apply(e, source, target);
        var battleSnapshotAfter = new BattleStateSnapshot(target.GetSnapshot());
        
        var effectResolved = new EffectResolved(e, source, new Single(target), battleSnapshotBefore, battleSnapshotAfter);

        var reactions = target.State.GetReactions(effectResolved);
        reactions.ForEach(r => r.Reaction.ActionSequence.CardActions.Actions.Where(a => a.Type == CardBattleActionType.Battle)
            .ForEach(be => AllEffects.Apply(be.BattleEffect, r.Source, r.Target)));
    }
}
