using System;

public class TeamEffect : Effect
{
    private readonly Action<EffectContext, TeamState> _action;

    public TeamEffect(Action<EffectContext, TeamState> action)
    {
        _action = action;
    }
    
    public void Apply(EffectContext ctx)
    {
        foreach (var teamState in ctx.TeamStates)
            _action(ctx, teamState);
    }
}