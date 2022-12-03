public class SetScopedVariable : Effect
{
    private readonly EffectData _e;

    public SetScopedVariable(EffectData e) => _e = e;

    public void Apply(EffectContext ctx)
    {
        var sourceSnapshot = ctx.Source.State.ToSnapshot();
        foreach (var m in ctx.Target.Members.GetConscious())
            ctx.ScopedData.SetVariable(_e.EffectScope.Value, Formula.EvaluateRaw(new FormulaContext(sourceSnapshot, m, ctx.XPaidAmount, ctx.ScopedData), _e.Formula));
    }
}