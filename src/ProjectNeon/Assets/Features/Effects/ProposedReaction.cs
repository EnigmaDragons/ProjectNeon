public class ProposedReaction
{
    public ReactionCardType Reaction { get; }
    public Member Source { get; }
    public Target Target { get; }

    public ProposedReaction(ReactionCardType a, Member source, Member target)
        : this(a, source, new Single(target)) {}
    
    public ProposedReaction(ReactionCardType a, Member source, Target target)
    {
        Reaction = a;
        Source = source;
        Target = target;
    }
}
