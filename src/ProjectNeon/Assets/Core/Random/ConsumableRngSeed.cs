public static class ConsumableRngSeed
{
    private static bool _loggingEnabled = true;
    private static OneTimeSeed NewSeed() => new OneTimeSeed(Rng.NewSeed());

    public static OneTimeSeed Current { get; private set; } = NewSeed();
    public static OneTimeSeed GenerateNew()
    {
        DebugLog("Generate New");
        return Current = NewSeed();
    }

    public static OneTimeSeed GetValid() =>
        Current.IsValid 
            ? Current 
            : GenerateNew();

    public static int Consume()
    {
        DebugLog("Consume");
        var seed = GetValid().Consume().Value;
        GenerateNew();
        return seed;
    }
    
    public static OneTimeSeed Init(int seed)
    {
        DebugLog($"Init {seed}");
        return Current = new OneTimeSeed(seed);
    }

    private static void DebugLog(string msg)
    {
        if (_loggingEnabled)
            Log.Info($"Consumable Rng Seed - {msg}");
    }
}

public class OneTimeSeed
{
    private readonly int _seed;
    private bool _consumed;

    public OneTimeSeed(int seed)
    {
        _seed = seed;
    }

    public bool IsValid => !_consumed;
    public Maybe<int> Peek => _consumed ? Maybe<int>.Missing() : _seed;

    public Maybe<int> Consume()
    {
        if (_consumed)
            return Maybe<int>.Missing();
        
        _consumed = true;
        return _seed;
    }
}
