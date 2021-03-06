﻿public class PerformerHasResourceCondition : ILogicFlow
{
    private readonly int _amount;
    private readonly string _resource;
    private readonly CardActionsData _effect;

    public PerformerHasResourceCondition(int amount, string resource, CardActionsData effect)
    {
        _amount = amount;
        _resource = resource;
        _effect = effect;
    }

    public IPayloadProvider Resolve(CardActionContext ctx)
    {
        if (ctx.Source.State[new InMemoryResourceType(_resource)] >= _amount)
            return _effect.Play(ctx);
        return new NoPayload();
    }
}
