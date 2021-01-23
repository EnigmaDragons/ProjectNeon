public class ProposedReaction
{
    public Maybe<ReactionCardType> ReactionCard { get; }
    public CardReactionSequence ReactionSequence { get; }
    public Member Source { get; }
    public Target Target { get; }

    public ProposedReaction(ReactionCardType a, Member source, Member target)
        : this(a, source, new Single(target)) {}
    
    public ProposedReaction(ReactionCardType a, Member source, Target target)
    {
        ReactionCard = a;
        Source = source;
        Target = target;
        ReactionSequence = a.ActionSequence;
    }
    
    public ProposedReaction(CardReactionSequence a, Member source, Target target)
    {
        ReactionCard = Maybe<ReactionCardType>.Missing();
        Source = source;
        Target = target;
        ReactionSequence = a;
    }
}
