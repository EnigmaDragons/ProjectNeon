
using UnityEngine;

public class TemporalStateMetadata
{
    public bool IsDebuff { get; }
    public int MaxUses { get; }
    public int MaxDurationTurns { get; }
    public bool IsIndefinite { get; }
    public StatusTag Tag { get; }

    public TemporalStateMetadata(bool isDebuff, int maxUses, int maxDurationTurns, bool isIndefinite, StatusTag tag)
    {
        if (MaxDurationTurns == 0 && !isIndefinite || MaxUses == 0)
            Log.Warn("Created Effect is permanently inactive");
        
        IsDebuff = isDebuff;
        MaxUses = maxUses;
        MaxDurationTurns = maxDurationTurns;
        IsIndefinite = isIndefinite;
        Tag = tag;
    }
    
    public static TemporalStateMetadata Indefinite(bool isDebuff, StatusTag tag)
        => new TemporalStateMetadata(isDebuff, -1, -1, true, tag);
    public static TemporalStateMetadata ForDuration(int numTurns, bool isDebuff, StatusTag tag) 
        => new TemporalStateMetadata(isDebuff, -1, numTurns, numTurns < 0, tag);
    public static TemporalStateMetadata DebuffForDuration(int numTurns, StatusTag tag) 
        => new TemporalStateMetadata(true, -1, numTurns, numTurns < 0, tag);
    public static TemporalStateMetadata BuffForDuration(int numTurns, StatusTag tag) 
        => new TemporalStateMetadata(false, -1, numTurns, numTurns < 0, tag);
}

public static class TemporalStateMetadataExtesions
{
    public static TemporalStateTracker CreateTracker(this TemporalStateMetadata m) => new TemporalStateTracker(m);

    public static TemporalStateMetadata ForSimpleDurationStatAdjustment(this EffectData e)
        => TemporalStateMetadata.ForDuration(e.NumberOfTurns, e.IntAmount < 0, StatusTag.None);
}
