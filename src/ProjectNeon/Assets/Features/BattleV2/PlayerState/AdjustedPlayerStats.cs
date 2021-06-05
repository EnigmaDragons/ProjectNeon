
public class AdjustedPlayerStats : TemporalStateBase, ITemporalPlayerState
{
    public IPlayerStats PlayerStats { get; }

    public override ITemporalState CloneOriginal() => new AdjustedPlayerStats(PlayerStats, Tracker.Metadata.MaxDurationTurns, Tracker.IsDebuff);
    
    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();

    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd() => new NoPayload();

    void ITemporalPlayerState.OnTurnStart() {}
    void ITemporalPlayerState.OnTurnEnd() => Tracker.AdvanceTurn();

    public AdjustedPlayerStats(IPlayerStats stats, int duration, bool isDebuff)
        : base(TemporalStateMetadata.ForDuration(-1, duration, isDebuff))
    {
        PlayerStats = stats;
    }
}
