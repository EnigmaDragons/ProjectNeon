using System;

public class TemporalStateTracker
{
    public TemporalStateMetadata Metadata { get; }

    private int _remainingTurns;
    private int _remainingUses;
    
    public TemporalStateTracker(TemporalStateMetadata metadata)
    {
        Metadata = metadata;
        _remainingTurns = metadata.MaxDurationTurns;
        _remainingUses = metadata.MaxUses;
    }

    public void AdvanceTurn() => _remainingTurns = Math.Max(0, _remainingTurns - 1);
    public void RecordUse() => _remainingUses = Math.Max(0, _remainingUses - 1);
    public bool IsDebuff => Metadata.IsDebuff;
    public bool IsActive => (_remainingTurns > 0 || Metadata.IsIndefinite) && (_remainingUses > 0 || Metadata.HasUnlimitedUses);
    public int RemainingTurns => _remainingTurns;
    public int RemainingUses => _remainingUses;
    public StatusDetail Status => Metadata.Status;
}
