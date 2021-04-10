
public class DamageOverTimeFormula : Effect
{
    private readonly EffectData _e;

    public DamageOverTimeFormula(EffectData e) => _e = e;

    public void Apply(EffectContext ctx)
    {
        var sourceSnapshot = ctx.Source.State.ToSnapshot();
        ctx.Target.Members.ForEach(m =>
        {
            var calculatedAmount = Formula.Evaluate(new FormulaContext(sourceSnapshot, m, ctx.XPaidAmount), _e.Formula).CeilingInt();
            m.State.ApplyTemporaryAdditive(new DamageOverTimeState(calculatedAmount, m, _e.NumberOfTurns.Value));
        });
    }
}
