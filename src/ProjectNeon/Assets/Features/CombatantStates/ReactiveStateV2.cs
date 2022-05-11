public interface ReactiveStateV2 : ITemporalState
{
    ReactionTimingWindow Timing { get; }
    Maybe<ProposedReaction> OnEffectResolved(EffectResolved e);
}
