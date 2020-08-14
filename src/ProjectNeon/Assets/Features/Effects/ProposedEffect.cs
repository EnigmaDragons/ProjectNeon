public class ProposedEffect
{
    public CardActionsData Action { get; }
    public Member Source { get; }
    public Target Target { get; }

    public ProposedEffect(CardActionsData a, Member source, Member target)
        : this(a, source, new Single(target)) {}
    
    public ProposedEffect(CardActionsData a, Member source, Target target)
    {
        Action = a;
        Source = source;
        Target = target;
    }
}
