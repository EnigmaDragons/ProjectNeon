
public class TemporalStateMetadata
{
    public bool IsDebuff { get; }
    public int MaxUses { get; }
    public int MaxDurationTurns { get; }
    public bool IsIndefinite { get; }

    public TemporalStateMetadata(bool isDebuff, int maxUses, int maxDurationTurns, bool isIndefinite)
    {
        IsDebuff = isDebuff;
        MaxUses = maxUses;
        MaxDurationTurns = maxDurationTurns;
        IsIndefinite = isIndefinite;
    }
    
    public static TemporalStateMetadata ForDuration(int numTurns, bool isDebuff) 
        => new TemporalStateMetadata(isDebuff, -1, numTurns, numTurns < 0);
    public static TemporalStateMetadata DebuffForDuration(int numTurns) 
        => new TemporalStateMetadata(true, -1, numTurns, numTurns < 0);
    public static TemporalStateMetadata BuffForDuration(int numTurns) 
        => new TemporalStateMetadata(false, -1, numTurns, numTurns < 0);
}

public static class TemporalStateMetadataExtesions
{
    public static TemporalStateTracker CreateTracker(this TemporalStateMetadata m) => new TemporalStateTracker(m);
}
