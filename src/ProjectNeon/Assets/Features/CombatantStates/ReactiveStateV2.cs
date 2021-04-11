public interface ReactiveStateV2 : ITemporalState
{
    Maybe<ProposedReaction> OnEffectResolved(EffectResolved e);
}
