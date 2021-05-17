using System.Linq;

public class TargetIsBelowPercentageLifeCondition : ILogicFlow
{
    private readonly float _percentage;
    private readonly CardActionsData _effect;

    public TargetIsBelowPercentageLifeCondition(float percentage, CardActionsData effect)
    {
        _percentage = percentage;
        _effect = effect;
    }

    public IPayloadProvider Resolve(CardActionContext ctx)
    {
        var applicableMembers = ctx.Target.Members.Where(x => x.State[TemporalStatType.HP] < x.State.MaxHp() * _percentage).ToArray();
        if (applicableMembers.Any())
            return _effect.Play(ctx.WithTarget(new Multiple(applicableMembers)));
        return new NoPayload();
    }
}
