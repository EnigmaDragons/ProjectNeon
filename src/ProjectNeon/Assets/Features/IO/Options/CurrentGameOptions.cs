using System;

public static class CurrentGameOptions
{
    private static Stored<PlayerOptionsData> _stored = new MemoryStored<PlayerOptionsData>(new PlayerOptionsData());
    private static PlayerOptionsData _current = new PlayerOptionsData();
    private static string _version = "Unknown";

    public static PlayerOptionsData Data => _current;
    
    public static void Init(string versionNumber, Stored<PlayerOptionsData> stored)
    {
        _version = versionNumber;
        _stored = stored;
        _current = _stored.Get();
        _current.UseAutoAdvance = false;
    }
    
    public static void Write(Func<PlayerOptionsData, PlayerOptionsData> transform)
    {
        _stored.Write(transform);
        _current = transform(_current);
        _current.UseAutoAdvance = false;
    }

    public static void Save() => _stored.Write(_ => _current);
    public static void Clear() => Write(_ => new PlayerOptionsData { VersionNumber = _version });
    
    [Obsolete]
    public static void SetAutoAdvance(bool shouldUse) => Write(o =>
    {
        o.UseAutoAdvance = shouldUse;
        return o;
    });

    public static void SetBattleSpeed(int factor) => Write(o =>
    {
        o.BattleSpeedFactor = factor;
        return o;
    });
}
