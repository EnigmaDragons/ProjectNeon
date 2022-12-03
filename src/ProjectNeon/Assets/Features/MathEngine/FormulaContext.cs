
public class FormulaContext
{
    public MemberStateSnapshot Source { get; }
    public Maybe<MemberState> Target { get; }
    public ResourceQuantity XAmountPaid { get; }
    public EffectScopedData ScopedData { get; }

    public FormulaContext(MemberStateSnapshot src, Member target, ResourceQuantity xAmountPaid, EffectScopedData scopedData)
        : this(src, target.State, xAmountPaid, scopedData) {}
    
    public FormulaContext(MemberStateSnapshot src, Maybe<MemberState> target, ResourceQuantity xAmountPaid, EffectScopedData scopedData)
    {
        Source = src;
        Target = target;
        XAmountPaid = xAmountPaid;
        ScopedData = scopedData;
    }
}
