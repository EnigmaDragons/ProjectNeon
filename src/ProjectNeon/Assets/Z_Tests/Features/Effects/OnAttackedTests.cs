using NUnit.Framework;
using UnityEngine;

public sealed class OnAttackedTests
{
    [Test]
    public void OnAttacked_ApplyTwice_OnlyGeneratesOneReaction()
    {
        AllEffects.InitBattleState(ScriptableObject.CreateInstance<BattleState>());
        var target = TestMembers.Any();
        
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

        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnAttacked,
            NumberOfTurns = new IntReference(3),
            ReactionSequence = reactionCardType
        }, target, target);
        
        Assert.AreEqual(1, target.State.ReactiveStates.Length);
    }
}
