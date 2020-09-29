
public class AtEndOfTurn : TemporalStateBase
{
    private readonly EffectContext _ctx;
    private readonly Member _member;
    private readonly CardActionsData _data;

    public AtEndOfTurn(EffectContext ctx, Member member, CardActionsData data, TemporalStateMetadata metadata)
        : base(metadata)
    {
        _ctx = ctx;
        _member = member;
        _data = data;
    }
    
    public override ITemporalState CloneOriginal() => new AtEndOfTurn(_ctx, _member, _data, Tracker.Metadata);
    public override IStats Stats { get; } = new StatAddends();
    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd()
    {
        if (!IsActive) 
            return new NoPayload();

        Tracker.AdvanceTurn();
        return _data.Play(new StatusEffectContext(_member));
    }
}
