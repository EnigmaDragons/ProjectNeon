public class GlobalEffectContext
{
    public CurrentGlobalEffects GlobalEffects { get; }

    public GlobalEffectContext(CurrentGlobalEffects globalEffects)
    {
        GlobalEffects = globalEffects;
    }
}
