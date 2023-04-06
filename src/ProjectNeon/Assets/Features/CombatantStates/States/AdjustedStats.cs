
public class AdjustedStats : TemporalStateBase
{
    public static AdjustedStats CreateIndefinite(int originatorId, IStats stats, bool isDebuff)
        => new AdjustedStats(stats, TemporalStateMetadata.Unlimited(originatorId, isDebuff));
    
    public AdjustedStats(IStats stats, TemporalStateMetadata metadata)
        : base(metadata)
    {
        _stats = new EvaluatedStats(stats, StatType.Power);
    }
    
    private EvaluatedStats _stats;
    public override IStats Stats => _stats;
    public override Maybe<int> Amount => Maybe<int>.Missing();
    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd()
    {
        Tracker.AdvanceTurn();
        return new NoPayload();
    }

    public override ITemporalState CloneOriginal() => new AdjustedStats(Stats, Tracker.Metadata);
}
