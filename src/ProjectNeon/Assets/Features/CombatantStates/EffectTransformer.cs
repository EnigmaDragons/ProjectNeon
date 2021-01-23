public interface EffectTransformer : ITemporalState
{
    EffectData Modify(EffectData effect, EffectContext context);
}