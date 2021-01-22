using NUnit.Framework;
using UnityEngine;

public sealed class ReactiveTriggerTests
{
    [Test, Ignore("Doesn't use new Async Reaction Resolution")]
    public void ReactiveTrigger_OnAttacked_GainedOneArmor()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1));

        var reactionCardType = TestCards.Reaction(
            ReactiveMember.Possessor, 
            ReactiveTargetScope.Self, 
            new EffectData { EffectType = EffectType.AdjustStatAdditivelyFormula, Formula = "1", EffectScope = new StringReference("Armor"), NumberOfTurns = new IntReference(-1) });

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            ReactionSequence = reactionCardType
        }, target, target);
        
        ReactiveTestUtilities.ApplyEffectAndReactions(new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1),
            EffectScope = new StringReference(ReactiveTargetScope.Self.ToString())
        }, attacker, target);
        
        Assert.AreEqual(1, target.State.Armor());
    }

    [Test, Ignore("Doesn't use new Async Reaction Resolution")]
    public void ReactiveTrigger_OnAttacked_AttackerHitForOneDamage()
    {
        var target = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Attack, 1));
        var attacker = TestMembers.Create(s => s.With(StatType.MaxHP, 10).With(StatType.Attack, 1));

        var reactionCardType = TestCards.Reaction(
            ReactiveMember.Originator, 
            ReactiveTargetScope.Attacker, 
            new EffectData { EffectType = EffectType.Attack, FloatAmount = new FloatReference(1) });

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            ReactionSequence = reactionCardType
        }, target, target);
        
        ReactiveTestUtilities.ApplyEffectAndReactions(new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1),
            EffectScope = new StringReference(ReactiveTargetScope.Self.ToString())
        }, attacker, target);
        
        Assert.AreEqual(9, attacker.CurrentHp());
    }
}
