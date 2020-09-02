
using System;

public class PlayerEffect : Effect
{
    private readonly Action<PlayerState> _action;

    public PlayerEffect(Action<PlayerState> action)
    {
        _action = action;
    }
    
    public void Apply(EffectContext ctx)
    {
        _action(ctx.PlayerState);
    }
}
