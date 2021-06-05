
using UnityEngine;

public class DamageOverTimeFormula : Effect
{
    private readonly EffectData _e;

    public DamageOverTimeFormula(EffectData e) => _e = e;

    public void Apply(EffectContext ctx)
    {
        var sourceSnapshot = ctx.Source.State.ToSnapshot();
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var calculatedAmount = Formula.Evaluate(new FormulaContext(sourceSnapshot, m, ctx.XPaidAmount), _e.Formula).CeilingInt();
            m.State.ApplyTemporaryAdditive(new DamageOverTimeState(ctx.Source.Id, calculatedAmount, m, Mathf.CeilToInt(Formula.Evaluate(ctx.SourceSnapshot.State, m.State, _e.DurationFormula, ctx.XPaidAmount))));
        });
    }
}
