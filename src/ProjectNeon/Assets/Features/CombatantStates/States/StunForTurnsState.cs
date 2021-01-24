
public sealed class StunForTurns : TemporalStateBase
{
    public StunForTurns(int duration)
        : base(TemporalStateMetadata.DebuffForDuration(duration, new StatusDetail(StatusTag.None))) {}
    
    public override IStats Stats => new StatAddends().With(TemporalStatType.TurnStun, Tracker.RemainingTurns);
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();
    public override ITemporalState CloneOriginal() => new StunForTurns(Tracker.Metadata.MaxDurationTurns);
    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd()
    {
        Tracker.AdvanceTurn();
        return new NoPayload();
    }
}
