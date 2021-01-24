
public class TemporalStateMetadata
{
    public bool IsDebuff { get; }
    public int MaxUses { get; }
    public int MaxDurationTurns { get; }
    public bool IsIndefinite { get; }
    public bool HasUnlimitedUses { get; }
    public StatusDetail Status { get; }

    public TemporalStateMetadata(bool isDebuff, int maxUses, int maxDurationTurns, StatusDetail status)
        : this(isDebuff, maxUses, maxDurationTurns, maxDurationTurns < 0, maxUses < 0, status) {}
    public TemporalStateMetadata(bool isDebuff, int maxUses, int maxDurationTurns, bool isIndefinite, bool hasUnlimitedUses, StatusDetail status)
    {
        if ((maxDurationTurns <= 0 && !isIndefinite) || (maxUses <= 0 && !hasUnlimitedUses))
            Log.Warn($"Created Effect is permanently inactive. Max Uses {maxUses}. Max Duration Turn {maxDurationTurns}.");
        
        IsDebuff = isDebuff;
        MaxUses = maxUses;
        MaxDurationTurns = maxDurationTurns;
        IsIndefinite = isIndefinite;
        HasUnlimitedUses = hasUnlimitedUses;
        Status = status;
    }
    
    public static TemporalStateMetadata Unlimited(bool isDebuff)
        => new TemporalStateMetadata(isDebuff, -1, -1, new StatusDetail(StatusTag.None));
    public static TemporalStateMetadata Unlimited(bool isDebuff, StatusDetail status)
        => new TemporalStateMetadata(isDebuff, -1, -1, status);
    public static TemporalStateMetadata ForDuration(int numTurns, bool isDebuff) 
        => new TemporalStateMetadata(isDebuff, -1, numTurns, new StatusDetail(StatusTag.None));
    public static TemporalStateMetadata ForDuration(int numTurns, bool isDebuff, StatusDetail status) 
        => new TemporalStateMetadata(isDebuff, -1, numTurns, status);
    public static TemporalStateMetadata DebuffForDuration(int numTurns) 
        => new TemporalStateMetadata(true, -1, numTurns, new StatusDetail(StatusTag.None));
    public static TemporalStateMetadata DebuffForDuration(int numTurns, StatusDetail status) 
        => new TemporalStateMetadata(true, -1, numTurns, status);
    public static TemporalStateMetadata BuffForDuration(int numTurns) 
        => new TemporalStateMetadata(false, -1, numTurns, new StatusDetail(StatusTag.None));
    public static TemporalStateMetadata BuffForDuration(int numTurns, StatusDetail status) 
        => new TemporalStateMetadata(false, -1, numTurns, status);
}

public static class TemporalStateMetadataExtesions
{
    public static TemporalStateTracker CreateTracker(this TemporalStateMetadata m) => new TemporalStateTracker(m);

    public static TemporalStateMetadata ForSimpleDurationStatAdjustment(this EffectData e)
        => TemporalStateMetadata.ForDuration(e.NumberOfTurns, e.IntAmount < 0, new StatusDetail(StatusTag.None, Maybe<string>.Missing()));
}
