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
    
    public void Perform(Member source, Target target, int amountPaid)
    {
        Message.Subscribe<SequenceFinished>(_ =>
        {
            Message.Unsubscribe(this);
            Message.Publish(new CardResolutionFinished());
        }, this);
        SequenceMessage.Queue(cardActions.PlayAsReaction(source, target, amountPaid));
    }
    
    public CardReactionSequence() {}
    public CardReactionSequence(ReactiveMember source, ReactiveTargetScope s, CardActionsData actions)
    {
        reactor = source;
        scope = s;
        cardActions = actions;
    }
}
