using System.Collections.Generic;
using System.Linq;

public class BattleReactions
{
    private readonly Queue<ProposedReaction> _instantReactions;
    private readonly Queue<ProposedReaction> _cardReactions;
    
    public bool Any => _instantReactions.Any() || _cardReactions.Any();
    public bool AnyReactionEffects => _instantReactions.Any();
    public bool AnyReactionCards => _cardReactions.Any();

    public BattleReactions(Queue<ProposedReaction> instantReactions, Queue<ProposedReaction> cardReactions)
    {
        _instantReactions = instantReactions;
        _cardReactions = cardReactions;
    }
    
    public void Enqueue(ProposedReaction[] reactions) => reactions.ForEach(Enqueue);

    public void Enqueue(ProposedReaction p)
    {
        if (p.ReactionCard.IsMissing)
            _instantReactions.Enqueue(p);
        else
            _cardReactions.Enqueue(p);
    }
    
    public void ResolveNextInstantReaction(IDictionary<int, Member> allBattleMembers)
    {
        var r = _instantReactions.Dequeue().WithPresentAndConsciousTargets(allBattleMembers);
        if (r.Target.Members.Any())
            r.ReactionSequence.Perform(r.Name, r.Source, r.Target, ResourceQuantity.None);
        else
            Message.Publish(new CardResolutionFinished(r.Name, -1, NextPlayedCardId.Get(), r.Source.Id));
    }

    public ProposedReaction DequeueNextReactionCard() => _cardReactions.Dequeue();
}
