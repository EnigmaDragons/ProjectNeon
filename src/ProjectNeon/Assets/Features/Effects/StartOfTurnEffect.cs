using UnityEngine;

public class StartOfTurnEffect : Effect
{
    private readonly EffectData _source;

    public StartOfTurnEffect(EffectData source) => _source = source;

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConsciousMembers(m => m.State.ApplyTemporaryAdditive(
            new AtStartOfTurnState(ctx, m, _source.ReferencedSequence,
                TemporalStateMetadata.ForDuration(ctx.Source.Id, Mathf.CeilToInt(Formula.Evaluate(ctx.SourceSnapshot.State, m.State, _source.DurationFormula, ctx.XPaidAmount)), _source.EffectScope.Value.Equals("Debuff"), _source.StatusDetail))));
    }
}
