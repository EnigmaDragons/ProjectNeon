using System;

public class TransferPrimaryResource : Effect
{
    private Func<EffectContext, Member, int> _calcAmount;

    public TransferPrimaryResource(Func<EffectContext, Member, int> calcAmount)
        => _calcAmount = calcAmount;
    
    public void Apply(EffectContext ctx)
    {
        var drainedResources = 0;
        foreach (var target in ctx.Target.Members)
        {
            var before = target.PrimaryResourceAmount();
            target.State.AdjustPrimaryResource(_calcAmount(ctx, target));
            var after = target.PrimaryResourceAmount();
            drainedResources += before - after;
        }
        ctx.Source.State.AdjustPrimaryResource(drainedResources);
    }
}