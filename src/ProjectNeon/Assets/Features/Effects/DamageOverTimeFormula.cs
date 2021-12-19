
public class DamageOverTimeFormula : Effect
{
    private readonly EffectData _e;

    public DamageOverTimeFormula(EffectData e) => _e = e;

    public void Apply(EffectContext ctx)
    {
        var sourceSnapshot = ctx.Source.State.ToSnapshot();
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var calculatedAmount = Formula.EvaluateToInt(new FormulaContext(sourceSnapshot, m, ctx.XPaidAmount), _e.Formula);
            m.State.ApplyTemporaryAdditive(new DamageOverTimeState(ctx.Source.Id, calculatedAmount, m, Formula.EvaluateToInt(ctx.SourceSnapshot.State, m.State, _e.DurationFormula, ctx.XPaidAmount)));
        });
    }
}
