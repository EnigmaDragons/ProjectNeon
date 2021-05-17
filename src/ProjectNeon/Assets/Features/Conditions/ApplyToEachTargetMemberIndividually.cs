using System.Linq;

public class ApplyToEachTargetMemberIndividually : ILogicFlow
{
    private readonly CardActionsData _referencedEffect;

    public ApplyToEachTargetMemberIndividually(CardActionsData referencedEffect)
    {
        _referencedEffect = referencedEffect;
    }

    public IPayloadProvider Resolve(CardActionContext ctx) 
        => ctx.Target.Members.Length < 2
            ? _referencedEffect.Play(ctx)
            : new MultiplePayloads(ctx.Target.Members
                .Select(m => ctx.WithTarget(new Single(m)))
                .Select(c => _referencedEffect.Play(c)));
}