
public static class LookupInitializer
{
    static LookupInitializer()
    {
        object _ = PlayerStatTypeExtensions.StatNames;
        _ = TemporalStatTypeExtensions.StatNames;
        _ = StatExtensions.StatNames;
        _ = StatusTagExtensions.StatNames;
        _ = AllEffects.Init;
        IntExtensions.Init();
        FloatExtensions.Init();
    }

    public static readonly bool Init = true;
}
