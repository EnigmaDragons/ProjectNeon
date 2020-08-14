public interface ReactiveStateV2 : ITemporalState
{
    Maybe<ProposedEffect> GetReaction();
}
