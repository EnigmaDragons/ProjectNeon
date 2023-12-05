public interface ReactiveStateV2 : ITemporalState
{
    ReactionTimingWindow Timing { get; }
    bool OnlyReactDuringCardPhases { get; }
    ProposedReaction[] OnEffectResolved(EffectResolved e);
}
