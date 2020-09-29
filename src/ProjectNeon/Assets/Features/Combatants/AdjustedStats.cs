
public class AdjustedStats : ITemporalState
{
    private readonly TemporalStateTracker _tracker;

    public IStats Stats { get; }
    public StatusTag Tag => _tracker.Metadata.Tag;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;
    public IPayloadProvider OnTurnStart() => new NoPayload();
    public IPayloadProvider OnTurnEnd()
    {
        _tracker.AdvanceTurn();
        return new NoPayload();
    }

    public ITemporalState CloneOriginal() => new AdjustedStats(Stats, _tracker.Metadata);

    public static AdjustedStats CreateIndefinite(IStats stats, bool isDebuff)
        => new AdjustedStats(stats, TemporalStateMetadata.Indefinite(isDebuff, StatusTag.None));
    
    public AdjustedStats(IStats stats, TemporalStateMetadata metadata)
    {
        _tracker = metadata.CreateTracker();
        Stats = stats;
    }
}
