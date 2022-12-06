using System;
using UnityEngine;

[Serializable]
public sealed class CardReactionSequence
{
    [SerializeField] private ReactiveMember reactor;
    [SerializeField] private ReactiveTargetScope scope;
    [SerializeField] public CardActionsData cardActions;

    public ReactiveMember Reactor => reactor;
    public ReactiveTargetScope Scope => scope;
    public CardActionsData CardActions => cardActions;

    public void Perform(string reactionName, Member source, Target target, ResourceQuantity xAmountPaid, ResourceQuantity amountPaid, ReactionTimingWindow timing)
    {
        MessageGroup.Start(cardActions.PlayAsReaction(source, target, xAmountPaid, amountPaid, reactionName, timing), 
            () => Message.Publish(new CardResolutionFinished(reactionName, -1, NextPlayedCardId.Get(), source.Id)));
    }

    public CardReactionSequence() {}
    public CardReactionSequence(ReactiveMember source, ReactiveTargetScope s, CardActionsData actions)
    {
        reactor = source;
        scope = s;
        cardActions = actions;
    }
}
