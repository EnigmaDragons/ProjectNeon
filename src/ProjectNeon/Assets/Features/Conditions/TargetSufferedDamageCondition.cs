﻿using System.Linq;

public class TargetSufferedDamageCondition : ILogicFlow
{
    private readonly CardActionsData _effect;

    public TargetSufferedDamageCondition(CardActionsData effect) => _effect = effect;
    
    public IPayloadProvider Resolve(CardActionContext ctx)
    {
        var applicableMembers = ctx.Target.Members.Where(x => ctx.BeforeState.Members[x.Id].State.Counters["HP"] > x.State[TemporalStatType.HP]).ToArray();
        if (applicableMembers.AnyNonAlloc())
            return _effect.Play(ctx.WithTarget(new Multiple(applicableMembers)));
        return new NoPayload();
    }
}
