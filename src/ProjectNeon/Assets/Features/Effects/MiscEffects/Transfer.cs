using System;

public class Transfer : Effect
{
    private readonly Maybe<TemporalStatType> _maybeStatType;
    private readonly Func<EffectContext, Member, int> _calcAmount;

    public Transfer(Maybe<TemporalStatType> maybeStatType, Func<EffectContext, Member, int> calcAmount)
    {
        _maybeStatType = maybeStatType;
        _calcAmount = calcAmount;
    }

    public void Apply(EffectContext ctx)
    {
        if (_maybeStatType.IsMissing)
        {
            Log.Error($"Transfer was attempted without a valid Temporal Stat Type by {ctx.Source.Name}");
            return;
        }

        var statType = _maybeStatType.Value;
        var drainedAmount = 0;
        foreach (var m in ctx.Target.Members.GetConscious())
        {
            var amount = -_calcAmount(ctx, m);
            if (amount < 0 && statType == TemporalStatType.HP || statType == TemporalStatType.Shield)
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Dodge, m.AsArray());
            else if (amount < 0)
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());
            
            if (ctx.Preventions.IsDodging(m) && statType == TemporalStatType.HP || statType == TemporalStatType.Shield)
                BattleLog.Write($"{m.Name} prevented Drain with a Dodge");
            else if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented Drain with an Aegis");
            else
                drainedAmount -= m.State.Adjust(statType, amount);
        }
        ctx.Source.State.Adjust(statType, drainedAmount);
        if (drainedAmount > 0)
            BattleLog.Write($"{ctx.Source} gained {drainedAmount} {statType}");
    }
}
