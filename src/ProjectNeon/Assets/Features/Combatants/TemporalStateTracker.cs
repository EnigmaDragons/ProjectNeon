
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

    public void AdvanceTurn() => _remainingTurns--;
    public void RecordUse() => _remainingUses--;
    public bool IsDebuff => Metadata.IsDebuff;
    public bool IsActive => Metadata.IsIndefinite || _remainingTurns > 0;
    public int RemainingTurns => _remainingTurns;
}
