
public class FormulaContext
{
    public MemberState Source { get; }
    public Maybe<MemberState> Target { get; }
    public ResourceQuantity XAmountPaid { get; }

    public FormulaContext(Member src, Member target, ResourceQuantity xAmountPaid)
        : this(src.State, target.State, xAmountPaid) {}
    
    public FormulaContext(MemberState src, Maybe<MemberState> target, ResourceQuantity xAmountPaid)
    {
        Source = src;
        Target = target;
        XAmountPaid = xAmountPaid;
    }
}
