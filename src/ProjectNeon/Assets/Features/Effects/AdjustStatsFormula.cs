
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

            var isDebuff = formulaAmount < 1;
            if (isDebuff) 
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());

            if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented {stat} debuff with an Aegis");
            else
            {
                BattleLog.Write($"{m.Name}'s {stat} is adjusted by x{formulaAmount}");
                var stats = new AdjustedStats(new StatMultipliers().WithRaw(stat, formulaAmount), _e.ForSimpleDurationStatAdjustment());
                m.State.ApplyTemporaryMultiplier(stats);
            }
        });
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
                BattleLog.Write($"{m.Name}'s {stat} is adjusted by {formulaAmount}");
                var stats = new AdjustedStats(new StatAddends().WithRaw(stat, formulaAmount),
                    _e.ForSimpleDurationStatAdjustment());
                m.State.ApplyTemporaryAdditive(stats);
            }
        });
    }
}
