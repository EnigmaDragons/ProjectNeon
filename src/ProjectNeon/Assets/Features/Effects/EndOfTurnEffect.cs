using UnityEngine;

public class EndOfTurnEffect : Effect
{
    private readonly EffectData _source;

    public EndOfTurnEffect(EffectData source)
    {
        _source = source;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConsciousMembers(m => m.State.ApplyTemporaryAdditive(
            new AtEndOfTurnState(ctx, m, _source.ReferencedSequence, 
                TemporalStateMetadata.ForDuration(ctx.Source.Id, Mathf.CeilToInt(Formula.Evaluate(ctx.SourceSnapshot.State, m.State, _source.DurationFormula, ctx.XPaidAmount)), _source.EffectScope.Value.Equals("Debuff"), new StatusDetail(_source.StatusTag)))));
    }
}
