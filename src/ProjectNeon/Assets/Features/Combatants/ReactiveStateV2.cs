public interface ReactiveStateV2 : ITemporalState
{
    StatusTag Tag { get; }
    Maybe<ProposedReaction> OnEffectResolved(EffectResolved e);
}
