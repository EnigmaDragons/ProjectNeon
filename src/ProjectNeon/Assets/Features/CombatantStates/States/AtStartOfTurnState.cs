using UnityEngine;

public class AtStartOfTurnState : TemporalStateBase
{
    private readonly EffectContext _ctx;
    private readonly Member _member;
    private readonly CardActionsData _data;
    private readonly StatusDetail _status;

    public AtStartOfTurnState(EffectContext ctx, Member member, CardActionsData data, TemporalStateMetadata metaData, StatusDetail status)
        : base(metaData)
    {
        _ctx = ctx;
        _member = member;
        _data = data;
        _status = status;
        CustomStatusText = status.CustomText;
    }
    
    public override ITemporalState CloneOriginal() => new AtStartOfTurnState(_ctx, _member, _data, Tracker.Metadata, _status);
    public override Maybe<string> CustomStatusText { get; } = Maybe<string>.Missing();
    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();

    public override IPayloadProvider OnTurnStart()
    {
        if (!IsActive) 
            return new NoPayload();

        Tracker.AdvanceTurn();
        return _data.Play(new StatusEffectContext(_ctx.Source, _member));
    }

    public override IPayloadProvider OnTurnEnd() => new NoPayload();
}
