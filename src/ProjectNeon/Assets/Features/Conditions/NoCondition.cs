public class NoCondition : ILogicFlow
{
    public IPayloadProvider Resolve(CardActionContext cardCtx)
        => new MultiplePayloads();
}
