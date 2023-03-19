
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
                TemporalStateMetadata.ForDuration(ctx.Source.Id, Formula.EvaluateToInt(ctx.SourceSnapshot.State, m.State, _source.DurationFormula, ctx.XPaidAmount, ctx.ScopedData), 
                    _source.EffectScope.Value.Equals("Debuff"), _source.StatusDetail))));
    }
}
