using System.Collections.Generic;
using System.Linq;

public class ProposedReaction
{
    public string Name => ReactionCard.Select(x => x.Name, "Reaction Effect");
    public Maybe<ReactionCardType> ReactionCard { get; }
    public CardReactionSequence ReactionSequence { get; }
    public Member Source { get; }
    public Target Target { get; }
    public ReactionTimingWindow Timing { get; }
    
    public ProposedReaction(ReactionCardType a, Member source, Target target, ReactionTimingWindow timing)
    {
        ReactionCard = Maybe<ReactionCardType>.Present(a);
        Source = source;
        Target = target;
        Timing = timing;
        ReactionSequence = a.ActionSequence;
    }
    
    public ProposedReaction(CardReactionSequence a, Member source, Target target, ReactionTimingWindow timing)
    {
        ReactionCard = Maybe<ReactionCardType>.Missing();
        Source = source;
        Target = target;
        Timing = timing;
        ReactionSequence = a;
    }

    private ProposedReaction(Maybe<ReactionCardType> maybeCard, CardReactionSequence a, Member source, Target target,
        ReactionTimingWindow timing)
    {
        ReactionCard = maybeCard;
        Source = source;
        Target = target;
        Timing = timing;
        ReactionSequence = a;
    }
    
    public ProposedReaction WithPresentAndConsciousTargets(IDictionary<int, Member> allMembers) 
        => new ProposedReaction(ReactionCard, ReactionSequence, Source, 
            new Multiple(Target.Members.Where(x => allMembers.Any(m => m.Key == x.Id) && x.IsConscious())), Timing);
}
