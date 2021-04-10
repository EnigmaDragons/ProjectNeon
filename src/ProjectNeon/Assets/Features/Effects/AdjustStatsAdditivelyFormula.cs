
public class AdjustStatsAdditivelyFormula : Effect
{
    private readonly EffectData _e;

    public AdjustStatsAdditivelyFormula(EffectData e) => _e = e;

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var stat = _e.EffectScope.Value.Equals("PrimaryStat")
                ? m.PrimaryStat().ToString()
                : _e.EffectScope;
            
            var formulaAmount = Formula.Evaluate(ctx.Source, m, _e.Formula, ctx.XPaidAmount).CeilingInt();
            
            var isDebuff = formulaAmount < 0;
            if (isDebuff) 
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());

            if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented {stat} debuff with an Aegis");
            else
            {
                BattleLog.Write($"{m.Name}'s {stat} is adjusted by {formulaAmount}");
                m.State.ApplyTemporaryAdditive(new AdjustedStats(new StatAddends().WithRaw(stat, formulaAmount), 
                    _e.ForSimpleDurationStatAdjustment()));
            }
        });
    }
}
