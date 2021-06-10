using System;

public static class CurrentGameData
{
    private static Stored<GameData> _stored = new MemoryStored<GameData>(new GameData());
    private static GameData _current = new GameData();

    public static bool HasActiveGame => _current?.IsInitialized ?? false;
    public static GameData Data => _current;
    
    public static void Init(Stored<GameData> stored)
    {
        _stored = stored;
        _current = _stored.Get();
    }

    public static void Write(Func<GameData, GameData> transform)
    {
        _stored.Write(transform);
        _current = transform(_current);
    }

    public static void Save() => _stored.Write(_ => _current);
}
