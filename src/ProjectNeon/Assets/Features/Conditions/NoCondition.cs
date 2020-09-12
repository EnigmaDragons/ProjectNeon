public class NoCondition : Condition
{
    public IPayloadProvider Resolve(CardActionContext cardCtx)
        => new MultiplePayloads();
}
