
public class TemporalStateMetadata
{
    public int OriginatorId { get; }
    public bool IsDebuff { get; }
    public int MaxUses { get; }
    public int MaxDurationTurns { get; }
    public bool HasUnlimitedDuration { get; }
    public bool HasUnlimitedUses { get; }
    public StatusDetail Status { get; }

    public TemporalStateMetadata(int originatorId, bool isDebuff, int maxUses, int maxDurationTurns, StatusDetail status)
        : this(originatorId, isDebuff, maxUses, maxDurationTurns, maxDurationTurns < 0, maxUses < 0, status) {}
    public TemporalStateMetadata(int originatorId, bool isDebuff, int maxUses, int maxDurationTurns, bool hasUnlimitedDuration, bool hasUnlimitedUses, StatusDetail status)
    {
        if ((maxDurationTurns <= 0 && !hasUnlimitedDuration) || (maxUses <= 0 && !hasUnlimitedUses))
            Log.Warn($"Created Effect is permanently inactive. Max Uses {maxUses}. Max Duration Turn {maxDurationTurns}.");

        OriginatorId = originatorId;
        IsDebuff = isDebuff;
        MaxUses = maxUses;
        MaxDurationTurns = maxDurationTurns;
        HasUnlimitedDuration = hasUnlimitedDuration;
        HasUnlimitedUses = hasUnlimitedUses;
        Status = status;
    }
    
    public static TemporalStateMetadata Unlimited(int originatorId, bool isDebuff)
        => new TemporalStateMetadata(originatorId, isDebuff, -1, -1, new StatusDetail(StatusTag.None));
    public static TemporalStateMetadata Unlimited(int originatorId, bool isDebuff, StatusDetail status)
        => new TemporalStateMetadata(originatorId, isDebuff, -1, -1, status);
    public static TemporalStateMetadata ForDuration(int originatorId, int numTurns, bool isDebuff) 
        => new TemporalStateMetadata(originatorId, isDebuff, -1, numTurns, new StatusDetail(StatusTag.None));
    public static TemporalStateMetadata ForDuration(int originatorId, int numTurns, bool isDebuff, StatusDetail status) 
        => new TemporalStateMetadata(originatorId, isDebuff, -1, numTurns, status);
    public static TemporalStateMetadata DebuffForDuration(int originatorId, int numTurns) 
        => new TemporalStateMetadata(originatorId, true, -1, numTurns, new StatusDetail(StatusTag.None));
    public static TemporalStateMetadata DebuffForDuration(int originatorId, int numTurns, StatusDetail status) 
        => new TemporalStateMetadata(originatorId, true, -1, numTurns, status);
    public static TemporalStateMetadata BuffForDuration(int originatorId, int numTurns) 
        => new TemporalStateMetadata(originatorId, false, -1, numTurns, new StatusDetail(StatusTag.None));
    public static TemporalStateMetadata BuffForDuration(int originatorId, int numTurns, StatusDetail status) 
        => new TemporalStateMetadata(originatorId, false, -1, numTurns, status);
}

public static class TemporalStateMetadataExtesions
{
    public static TemporalStateTracker CreateTracker(this TemporalStateMetadata m) => new TemporalStateTracker(m);

    public static TemporalStateMetadata ForSimpleDurationStatAdjustment(this EffectData e, int duration, int originatorId)
        => TemporalStateMetadata.ForDuration(originatorId, duration, e.IntAmount < 0, new StatusDetail(StatusTag.None, Maybe<string>.Missing()));
}
