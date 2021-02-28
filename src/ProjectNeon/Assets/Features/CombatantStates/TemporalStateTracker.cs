using System;

public class TemporalStateTracker
{
    private const bool _performDebugLogging = true;
    
    public TemporalStateMetadata Metadata { get; }

    private int _remainingTurns;
    private int _remainingUses;
    
    public TemporalStateTracker(TemporalStateMetadata metadata)
    {
        Metadata = metadata;
        _remainingTurns = metadata.MaxDurationTurns;
        _remainingUses = metadata.MaxUses;
    }

    public void AdvanceTurn() => DebugLog(() => _remainingTurns = !Metadata.HasUnlimitedDuration ? Math.Max(0, _remainingTurns - 1) : -1);
    public void RecordUse() => DebugLog(() => _remainingUses = !Metadata.HasUnlimitedUses ? Math.Max(0, _remainingUses - 1) : -1);
    public void UndoUse() => DebugLog(() => _remainingUses = Metadata.HasUnlimitedUses ? _remainingUses :  _remainingUses + 1);
    
    public bool IsDebuff => Metadata.IsDebuff;
    public bool IsActive => (_remainingTurns > 0 || Metadata.HasUnlimitedDuration) && (_remainingUses > 0 || Metadata.HasUnlimitedUses);
    public int RemainingTurns => _remainingTurns;
    public int RemainingUses => _remainingUses;
    public StatusDetail Status => Metadata.Status;
    
    private void DebugLog(Action a)
    {
        a();
        DevLog.Write($"Remaining Uses {_remainingUses} Turns {_remainingTurns}");
    }
}
