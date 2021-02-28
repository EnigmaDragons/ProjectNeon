using System;
using UnityEngine;

[Serializable]
public sealed class CardReactionSequence
{
    [SerializeField] private ReactiveMember reactor;
    [SerializeField] private ReactiveTargetScope scope;
    [SerializeField] private AvoidanceType avoidance;
    [SerializeField] public CardActionsData cardActions;

    public ReactiveMember Reactor => reactor;
    public ReactiveTargetScope Scope => scope;
    public CardActionsData CardActions => cardActions;

    public void Perform(string reactionName, Member source, Target target, ResourceQuantity xAmountPaid)
    {
        MessageGroup.Start(cardActions.PlayAsReaction(source, target, xAmountPaid, reactionName, avoidance), 
            () => Message.Publish(new CardResolutionFinished(source.Id, false)));
    }

    public CardReactionSequence() {}
    public CardReactionSequence(ReactiveMember source, ReactiveTargetScope s, CardActionsData actions)
    {
        reactor = source;
        scope = s;
        cardActions = actions;
    }
}
