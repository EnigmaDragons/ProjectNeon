using NUnit.Framework;
using UnityEngine;

public sealed class OnAttackedTests
{
    [Test]
    public void OnAttacked_ApplyTwice_OnlyGeneratesOneReaction()
    {
        var target = TestMembers.Any();
        
        var reactionCardType = TestCards.ReactionCard(
            ReactiveMember.Originator, 
            ReactiveTargetScope.Source, 
            new EffectData { EffectType = EffectType.Attack, FloatAmount = new FloatReference(1) });
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            FloatAmount = new FloatReference(-1),
            ReactionSequence = reactionCardType
        }, target, target);

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            FloatAmount = new FloatReference(-1),
            ReactionSequence = reactionCardType
        }, target, target);
        
        Assert.AreEqual(1, target.State.ReactiveStates.Length);
    }
}
