using System;
using UnityEngine;

public class SimpleCondition : ILogicFlow
{
    private readonly CardActionsData _effect;
    private readonly Func<CardActionContext, bool> _condition;

    public SimpleCondition(CardActionsData effect, Func<CardActionContext, bool> condition)
    {
        _effect = effect;
        _condition = condition;
    }
    
    public IPayloadProvider Resolve(CardActionContext ctx)
    {
        if (_condition(ctx))
            return _effect.Play(ctx);
        return new NoPayload();
    }
}
