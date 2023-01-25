public interface ReactiveStateV2 : ITemporalState
{
    ReactionTimingWindow Timing { get; }
    bool OnlyReactDuringCardPhases { get; }
    Maybe<ProposedReaction> OnEffectResolved(EffectResolved e);
}
