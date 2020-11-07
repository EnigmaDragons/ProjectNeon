
public class FormulaContext
{
    public MemberState Source { get; }
    public Maybe<MemberState> Target { get; }

    public FormulaContext(Member src, Member target)
        : this(src.State, target.State) {}
    
    public FormulaContext(MemberState src, Maybe<MemberState> target)
    {
        Source = src;
        Target = target;
    }
}
