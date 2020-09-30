
public class AdjustedStats : TemporalStateBase
{
    public static AdjustedStats CreateIndefinite(IStats stats, bool isDebuff)
        => new AdjustedStats(stats, TemporalStateMetadata.Indefinite(isDebuff, StatusTag.None));
    
    public AdjustedStats(IStats stats, TemporalStateMetadata metadata)
        : base(metadata)
    {
        Stats = stats;
    }
    
    public override IStats Stats { get; }
    public override Maybe<int> Amount => Maybe<int>.Missing();
    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd()
    {
        Tracker.AdvanceTurn();
        return new NoPayload();
    }

    public override ITemporalState CloneOriginal() => new AdjustedStats(Stats, Tracker.Metadata);
}
