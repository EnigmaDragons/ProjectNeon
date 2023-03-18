using System;
using System.Linq;

public class CurrentProgressionData
{
    private static Stored<ProgressionData> _stored = new MemoryStored<ProgressionData>(new ProgressionData());
    private static ProgressionData _current = new ProgressionData();
    private static string _version;

    public static ProgressionData Data => _current;

    public static void Init(Stored<ProgressionData> stored, string version)
    {
        _stored = stored;
        _current = _stored.Get();
        _version = version;
    }
    
    public static void RecordCompletedAdventure(int adventureId, int difficulty, int bossId, int[] heroes)
    {
        Write(d =>
        {
            d.CompletedAdventureIds = d.CompletedAdventureIds.Concat(adventureId).Distinct().ToArray();
            heroes.ForEach(heroId => d.Record(new AdventureCompletionRecord { AdventureId = adventureId, HeroId = heroId, Difficulty = difficulty, BossId = bossId, Version = _version }));
            if (adventureId != 10)
                d.RunsFinished += 1;
            return d;
        });
    }
    
    public static void Mutate(Action<ProgressionData> mutate)
    {
        Write(d =>
        {
            mutate(d);
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
