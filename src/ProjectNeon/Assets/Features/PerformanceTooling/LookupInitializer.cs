
public static class LookupInitializer
{
    static LookupInitializer()
    {
        object _ = PlayerStatTypeExtensions.StatNames;
        _ = TemporalStatTypeExtensions.StatNames;
        _ = StatExtensions.StatNames;
        _ = StatusTagExtensions.StatNames;
        _ = AllEffects.Init;
        _ = BattleV2PhaseExtensions.Init;
        _ = MemberStateSnapshotExtensions.Init;
        _ = WithMinimumStatsExtensions.Init;
        _ = WithPowerCountingAsStatExtensions.Init;
        _ = WholeNumberStatExtensions.Init;
        IntExtensions.Init();
        FloatExtensions.Init();
    }

    public static readonly bool Init = true;
}
