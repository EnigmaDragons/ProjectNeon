using System.Linq;

public sealed class TargetSlainCondition : ILogicFlow
{
    private readonly CardActionsData _effect;

    public TargetSlainCondition(CardActionsData effect)
    {
        _effect = effect;
    }

    public IPayloadProvider Resolve(CardActionContext ctx)
    {        
        if (ctx.Target.Members.Any(m => !m.IsConscious()))
            return _effect.Play(ctx);
        return new NoPayload();
    }
}
