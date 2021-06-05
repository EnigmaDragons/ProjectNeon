
public sealed class DisableForTurns : TemporalStateBase
{
    public DisableForTurns(int originatorId, int duration)
        : base(TemporalStateMetadata.DebuffForDuration(originatorId, duration, new StatusDetail(StatusTag.None))) {}
    
    public override IStats Stats => new StatAddends().With(TemporalStatType.Disabled, Tracker.RemainingTurns);
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();
    public override ITemporalState CloneOriginal() => new DisableForTurns(Tracker.Metadata.OriginatorId, Tracker.Metadata.MaxDurationTurns);
    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd()
    {
        Tracker.AdvanceTurn();
        return new NoPayload();
    }
}
