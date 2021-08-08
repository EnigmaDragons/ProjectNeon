using System.Threading;

public static class NextPlayedCardId
{
    private static int _nextPlayedCardId = 0;
    
    public static void Reset() => Interlocked.Exchange(ref _nextPlayedCardId, 0);
    
    public static int Get()
    {
        if (_nextPlayedCardId +1 == int.MaxValue)
            Reset();
        return Interlocked.Increment(ref _nextPlayedCardId);
    }
}