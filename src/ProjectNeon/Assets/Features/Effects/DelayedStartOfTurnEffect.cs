public class DelayedStartOfTurnEffect : Effect
{
    private readonly EffectData _source;

    public DelayedStartOfTurnEffect(EffectData source) => _source = source;

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConsciousMembers(m => m.State.ApplyTemporaryAdditive(
            new DelayedStartOfTurnState(ctx, m, _source.ReferencedSequence, 
                TemporalStateMetadata.ForDuration(_source.NumberOfTurns, _source.EffectScope.Value.Equals("Debuff"), _source.StatusTag))));
    }
}