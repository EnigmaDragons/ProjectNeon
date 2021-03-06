
public abstract class TemporalStateBase : ITemporalState
{
    protected readonly TemporalStateTracker Tracker;

    public int OriginatorId => Tracker.Metadata.OriginatorId;
    public StatusDetail Status => Tracker.Status;
    public bool IsDebuff => Tracker.IsDebuff;
    public bool IsActive => Tracker.IsActive;
    public Maybe<int> RemainingTurns => Tracker.RemainingTurns;

    protected TemporalStateBase(TemporalStateMetadata metadata) 
        => Tracker = metadata.CreateTracker();

    public abstract IStats Stats { get; }
    public abstract Maybe<int> Amount { get; }
    public abstract IPayloadProvider OnTurnStart();
    public abstract IPayloadProvider OnTurnEnd();
    public abstract ITemporalState CloneOriginal();
}
