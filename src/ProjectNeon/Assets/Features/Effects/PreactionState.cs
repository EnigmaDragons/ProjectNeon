
public interface PreactionState : ITemporalState
{
    Maybe<ProposedReaction> OnEffectQueued(EffectData e, EffectContext ctx);
}