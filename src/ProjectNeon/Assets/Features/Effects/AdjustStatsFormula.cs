
using System;
using UnityEngine;

public class AdjustStatsFormula : Effect
{
    private readonly EffectData _e;
    private readonly bool _multiplicative;

    public AdjustStatsFormula(EffectData e, bool multiplicative)
    {
        _e = e;
        _multiplicative = multiplicative;
    }

    public void Apply(EffectContext ctx)
    {
        if (_multiplicative)
            ApplyMultiplicative(ctx);
        else
            ApplyAdditive(ctx);
    }

    private void ApplyMultiplicative(EffectContext ctx)
    {
        var sourceSnapshot = ctx.Source.GetSnapshot().State;
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var stat = _e.EffectScope.Value.Equals("PrimaryStat")
                ? m.PrimaryStat().ToString()
                : _e.EffectScope;

            var formulaAmount = Formula.Evaluate(sourceSnapshot, m.State, _e.Formula, ctx.XPaidAmount);

            var isDebuff = StatExtensions.IsPositive(stat) ? formulaAmount < 1 : formulaAmount > 0;
            if (isDebuff) 
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());

            if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented {stat} debuff with an Aegis");
            else
            {
                BattleLog.Write($"{m.Name}'s {stat} is adjusted by x{formulaAmount}");
                var stats = new AdjustedStats(new StatMultipliers().WithRaw(stat, formulaAmount), _e.ForSimpleDurationStatAdjustment(ctx.Source.Id,
                    Mathf.CeilToInt(Formula.Evaluate(ctx.SourceSnapshot.State, m.State, _e.DurationFormula, ctx.XPaidAmount))));
                m.State.ApplyTemporaryMultiplier(stats);
            }
        });
    }

    private int CurrentStatAmount(Member m, string stat)
    {
        if (Enum.TryParse<StatType>(stat, out var statType))
            return m.State[statType].CeilingInt();
        if (Enum.TryParse<TemporalStatType>(stat, out var temporalStatType))
            return m.State[temporalStatType].CeilingInt();
        return 0;
    }
    
    private void ApplyAdditive(EffectContext ctx)
    {
        var sourceSnapshot = ctx.Source.GetSnapshot().State;
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var stat = _e.EffectScope.Value.Equals("PrimaryStat")
                ? m.PrimaryStat().ToString()
                : _e.EffectScope;

            var formulaAmount = Formula.Evaluate(sourceSnapshot, m.State, _e.Formula, ctx.XPaidAmount).CeilingInt();
            var isDebuff = formulaAmount < 0;
            if (isDebuff)
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());

            if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented {stat} debuff with an Aegis");
            else
            {                
                var finalAmount = isDebuff 
                    ? Mathf.Clamp(formulaAmount, -CurrentStatAmount(m, stat), 0) 
                    : formulaAmount;
                BattleLog.Write($"{m.Name}'s {stat} is adjusted by {finalAmount}");
                var stats = new AdjustedStats(new StatAddends().WithRaw(stat, finalAmount),
                    _e.ForSimpleDurationStatAdjustment(ctx.Source.Id, Mathf.CeilToInt(Formula.Evaluate(ctx.SourceSnapshot.State, m.State, _e.DurationFormula, ctx.XPaidAmount))));
                m.State.ApplyTemporaryAdditive(stats);
            }
        });
    }
}
