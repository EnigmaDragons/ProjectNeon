using Features.CombatantStates.Reactions;
using NUnit.Framework;

public class ReactOnTests
{
    [Test]
    public void ReactOn_OnStunned_Works()
    {
        var attacker = TestMembers.Named("Electroblade");
        var target = TestMembers.Named("Robot");

        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.ReactWithEffect,
            ReactionConditionType = ReactionConditionType.OnCausedStun,
            FloatAmount = new FloatReference(-1),
            NumberOfTurns = new IntReference(-1),
//            ReactionEffect = TestCards.ReactionEffect(
            ReactionSequence = TestCards.ReactionCard(
                    ReactiveMember.Possessor, 
                    ReactiveTargetScope.Target, 
                    new EffectData
                    {
                        EffectType = EffectType.AdjustCounter, 
                        EffectScope = new StringReference("Evade"), 
                        BaseAmount = new IntReference(1)
                    })
        }, attacker, attacker);
        
        TestEffects.Apply(new EffectData
        {
            EffectType = EffectType.StunForNumberOfCards,
            FloatAmount = new FloatReference(1)
        }, attacker, target);
        
        Assert.AreEqual(1, target.State[TemporalStatType.CardStun], "Card Stun wasn't applied");
        Assert.AreEqual(1, target.State[TemporalStatType.Evade], "Reaction didn't trigger");
    }
}
