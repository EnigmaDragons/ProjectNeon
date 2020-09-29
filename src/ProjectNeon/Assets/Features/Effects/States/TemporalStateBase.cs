
public abstract class TemporalStateBase : ITemporalState
{
    protected readonly TemporalStateTracker Tracker;

    public StatusTag Tag => Tracker.Metadata.Tag;
    public bool IsDebuff => Tracker.IsDebuff;
    public bool IsActive => Tracker.IsActive;

    public TemporalStateBase(TemporalStateMetadata metadata) => Tracker = metadata.CreateTracker();

    public abstract IStats Stats { get; }
    public abstract IPayloadProvider OnTurnStart();
    public abstract IPayloadProvider OnTurnEnd();
    public abstract ITemporalState CloneOriginal();
}
