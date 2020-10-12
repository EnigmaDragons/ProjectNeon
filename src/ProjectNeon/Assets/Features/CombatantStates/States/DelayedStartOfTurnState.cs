public class DelayedStartOfTurnState : TemporalStateBase
{
    private readonly EffectContext _ctx;
    private readonly Member _member;
    private readonly CardActionsData _data;

    public DelayedStartOfTurnState(EffectContext ctx, Member member, CardActionsData data, TemporalStateMetadata metaData)
        : base(metaData)
    {
        _ctx = ctx;
        _member = member;
        _data = data;
    }

    public override ITemporalState CloneOriginal() => new DelayedStartOfTurnState(_ctx, _member, _data, Tracker.Metadata);
    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();

    public override IPayloadProvider OnTurnStart()
    {
        if (!IsActive) 
            return new NoPayload();

        Tracker.AdvanceTurn();
        return Tracker.IsActive ? new NoPayload() : _data.Play(new StatusEffectContext(_member));
    }

    public override IPayloadProvider OnTurnEnd() => new NoPayload();
}