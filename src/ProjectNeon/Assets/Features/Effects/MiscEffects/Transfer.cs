using System;

public class Transfer : Effect
{
    private readonly TemporalStatType _statType;
    private readonly Func<EffectContext, Member, int> _calcAmount;

    public Transfer(TemporalStatType statType, Func<EffectContext, Member, int> calcAmount)
    {
        _statType = statType;
        _calcAmount = calcAmount;
    }

    public void Apply(EffectContext ctx)
    {
        var drainedAmount = 0;
        foreach (var m in ctx.Target.Members.GetConscious())
        {
            var amount = -_calcAmount(ctx, m);
            if (amount < 0 && _statType == TemporalStatType.HP || _statType == TemporalStatType.Shield)
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Dodge, m.AsArray());
            else if (amount < 0)
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());
            
            if (ctx.Preventions.IsDodging(m) && _statType == TemporalStatType.HP || _statType == TemporalStatType.Shield)
                BattleLog.Write($"{m.Name} prevented Drain with a Dodge");
            else if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented Drain with an Aegis");
            else
                drainedAmount -= m.State.Adjust(_statType, amount);
        }
        ctx.Source.State.Adjust(_statType, drainedAmount);
        if (drainedAmount > 0)
            BattleLog.Write($"{ctx.Source} gained {drainedAmount} {_statType}");
    }
}
