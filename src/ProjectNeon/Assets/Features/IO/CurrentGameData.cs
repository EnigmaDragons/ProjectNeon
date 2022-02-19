using System;

public static class CurrentGameData
{
    private static Stored<GameData> _stored = new MemoryStored<GameData>(new GameData());
    private static GameData _current = new GameData();
    private static string _version = "Unknown";

    public static bool HasActiveGame => _current?.IsInitialized ?? false;
    public static GameData Data => _current;
    public static bool SaveMatchesCurrentVersion => !HasActiveGame || SaveGameVersion == _version;
    public static string SaveGameVersion => HasActiveGame ? Data.VersionNumber : _version;
    public static string GameVersion => _version;

    public static void Init(string versionNumber, Stored<GameData> stored)
    {
        _version = versionNumber;
        _stored = stored;
        _current = _stored.Get();
    }
    
    public static void Write(Func<GameData, GameData> transform)
    {
        _stored.Write(g => transform(Upversion(g)));
        _current = transform((Upversion(_current)));
    }

    public static void Save() => _stored.Write(_ => _current);
    public static void Clear() => Write(_ => new GameData {VersionNumber = _version});

    private static GameData Upversion(GameData g)
    {
        if (_version != "Unknown")
            g.VersionNumber = _version;
        return g;
    }
}
