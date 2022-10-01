using System;

public class PermanentStats
{
    private static Stored<PermanentStatsData> _stored = new MemoryStored<PermanentStatsData>(new PermanentStatsData());
    private static PermanentStatsData _current = new PermanentStatsData();

    public static PermanentStatsData Data => _current;

    public static void Init(Stored<PermanentStatsData> stored)
    {
        _stored = stored;
        _current = _stored.Get();
    }

    public static void Mutate(Action<PermanentStatsData> mutate)
    {
        Write(d =>
        {
            mutate(d);
            return d;
        });
    }
    
    public static void Write(Func<PermanentStatsData, PermanentStatsData> transform)
    {
        _stored.Write(transform);
        _current = transform(_current);
    }
}
