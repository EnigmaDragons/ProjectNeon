public interface ReactiveStateV2 : ITemporalState
{
    Maybe<ProposedEffect> OnEffectResolved(EffectResolved e);
}
