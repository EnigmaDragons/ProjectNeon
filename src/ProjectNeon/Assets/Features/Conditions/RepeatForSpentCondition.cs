using System.Linq;

public class RepeatForSpentCondition : Condition
{
    private readonly CardActionsData _effect;

    public RepeatForSpentCondition(CardActionsData effect) => _effect = effect;

    public IPayloadProvider Resolve(CardActionContext ctx)
        => new MultiplePayloads(Enumerable.Range(0, ctx.XAmountPaid.Amount).Select(x => _effect.Play(ctx)));
}
