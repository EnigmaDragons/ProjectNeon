
public class FormulaContext
{
    public MemberState Source { get; }
    public Maybe<MemberState> Target { get; }
    public int XPaidAmount { get; }

    public FormulaContext(Member src, Member target, int xPaidAmount)
        : this(src.State, target.State, xPaidAmount) {}
    
    public FormulaContext(MemberState src, Maybe<MemberState> target, int xPaidAmount)
    {
        Source = src;
        Target = target;
        XPaidAmount = xPaidAmount;
    }
}
