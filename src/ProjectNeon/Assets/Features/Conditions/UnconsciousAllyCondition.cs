using System.Linq;

public class UnconsciousAllyCondition : Condition
{
    private readonly CardActionsData _effect;

    public UnconsciousAllyCondition(CardActionsData effect)
    {
        _effect = effect;
    }
    
    public IPayloadProvider Resolve(CardActionContext ctx)
    {
        var team = ctx.Source.TeamType;
        if (ctx.BeforeState.Members.Values.Where(m => m.TeamType == team).Any(m => m.IsUnconscious()))
            return _effect.Play(ctx);
        return new NoPayload();
    }
}
