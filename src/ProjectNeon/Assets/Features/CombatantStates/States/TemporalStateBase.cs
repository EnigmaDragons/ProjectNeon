
public abstract class TemporalStateBase : ITemporalState
{
    protected readonly TemporalStateTracker Tracker;

    public StatusDetail Status => new StatusDetail(Tag, CustomStatusText);
    public StatusTag Tag => Tracker.Metadata.Tag;
    public bool IsDebuff => Tracker.IsDebuff;
    public bool IsActive => Tracker.IsActive;
    public Maybe<int> RemainingTurns => Tracker.RemainingTurns;

    public TemporalStateBase(TemporalStateMetadata metadata) => Tracker = metadata.CreateTracker();

    public abstract Maybe<string> CustomStatusText { get; }
    public abstract IStats Stats { get; }
    public abstract Maybe<int> Amount { get; }
    public abstract IPayloadProvider OnTurnStart();
    public abstract IPayloadProvider OnTurnEnd();
    public abstract ITemporalState CloneOriginal();
}
