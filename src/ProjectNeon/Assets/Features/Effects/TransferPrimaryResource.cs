using System;

public class TransferPrimaryResource : Effect
{
    private readonly Func<EffectContext, Member, int> _calcAmount;

    public TransferPrimaryResource(Func<EffectContext, Member, int> calcAmount)
        => _calcAmount = calcAmount;
    
    public void Apply(EffectContext ctx)
    {
        var drainedResources = 0;
        foreach (var m in ctx.Target.Members.GetConscious())
        {
            var amount = _calcAmount(ctx, m);
            if (amount < 0)
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());
            
            if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented Resource Drain  with an Aegis");
            else
            {
                var before = m.PrimaryResourceAmount();
                m.State.AdjustPrimaryResource(amount);
                var after = m.PrimaryResourceAmount();
                drainedResources += before - after;
            }
        }
        ctx.Source.State.AdjustPrimaryResource(drainedResources);
        if (drainedResources > 0)
            BattleLog.Write($"{ctx.Source} gained {drainedResources} {ctx.Source.PrimaryResource().ResourceType}");
    }
}