
public class FormulaContext
{
    public MemberStateSnapshot Source { get; }
    public Maybe<MemberState> Target { get; }
    public ResourceQuantity XAmountPaid { get; }

    public FormulaContext(MemberStateSnapshot src, Member target, ResourceQuantity xAmountPaid)
        : this(src, target.State, xAmountPaid) {}
    
    public FormulaContext(MemberStateSnapshot src, Maybe<MemberState> target, ResourceQuantity xAmountPaid)
    {
        Source = src;
        Target = target;
        XAmountPaid = xAmountPaid;
    }
}
