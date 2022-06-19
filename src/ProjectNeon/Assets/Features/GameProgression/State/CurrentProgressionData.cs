using System;
using System.Linq;

public class CurrentProgressionData
{
    private static Stored<ProgressionData> _stored = new MemoryStored<ProgressionData>(new ProgressionData());
    private static ProgressionData _current = new ProgressionData();

    public static ProgressionData Data => _current;

    public static void Init(Stored<ProgressionData> stored)
    {
        _stored = stored;
        _current = _stored.Get();
    }

    public static void RecordCompletedAdventure(int adventureId)
    {
        Write(d =>
        {
            d.CompletedAdventureIds = d.CompletedAdventureIds.Concat(adventureId).Distinct().ToArray();
            return d;
        });
    }
    
    public static void Write(Func<ProgressionData, ProgressionData> transform)
    {
        _stored.Write(transform);
        _current = transform(_current);
    }

    public static void Clear() => Write(_ => new ProgressionData());
}
