
public sealed class HealOverTimeFormula : Effect
{
    private readonly EffectData _e;

    public HealOverTimeFormula(EffectData e)
    {
        _e = e;
    }
    
    public void Apply(EffectContext ctx)
    {
        var amount = Formula.EvaluateToInt(ctx.SourceStateSnapshot, _e.Formula, ctx.XPaidAmount, ctx.ScopedData);
        var numTurns = Formula.EvaluateToInt(ctx.SourceStateSnapshot, _e.DurationFormula, ctx.XPaidAmount, ctx.ScopedData);
        ctx.Target.Members.ForEach(x => x.State.ApplyTemporaryAdditive(new HealOverTimeState(ctx.Source.Id, amount, x, numTurns)));
    }
}
