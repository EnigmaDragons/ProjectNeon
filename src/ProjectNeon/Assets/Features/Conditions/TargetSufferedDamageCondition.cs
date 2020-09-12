using System.Linq;

public class TargetSufferedDamageCondition : Condition
{
    private readonly CardActionsData _effect;

    public TargetSufferedDamageCondition(CardActionsData effect) => _effect = effect;
    
    public IPayloadProvider Resolve(CardActionContext ctx)
    {
        var applicableMembers = ctx.Target.Members.Where(x => ctx.BeforeState.Members[x.Id].State.Counters["HP"] > x.State[TemporalStatType.HP]).ToArray();
        if (applicableMembers.Any())
            return _effect.Play(ctx.WithTarget(new Multiple(applicableMembers)));
        return new NoPayload();
    }
}
