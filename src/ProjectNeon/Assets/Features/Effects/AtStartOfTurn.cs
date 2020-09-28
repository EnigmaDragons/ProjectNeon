
public class StartOfTurnEffect : Effect
{
    private readonly EffectData _source;

    public StartOfTurnEffect(EffectData source) => _source = source;

    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConsciousMembers(m => m.State.ApplyTemporaryAdditive(
            new AtStartOfTurn(ctx, m, _source.ReferencedSequence, 
                TemporalStateMetadata.ForDuration(_source.NumberOfTurns, _source.EffectScope.Value.Equals("Debuff")))));
    }
}

public class AtStartOfTurn : ITemporalState
{
    private readonly TemporalStateTracker _tracker;
    private readonly EffectContext _ctx;
    private readonly Member _member;
    private readonly CardActionsData _data;

    public IStats Stats { get; } = new StatAddends();
    public StatusTag Tag => StatusTag.None;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;

    public AtStartOfTurn(EffectContext ctx, Member member, CardActionsData data, TemporalStateMetadata metaData)
    {
        _ctx = ctx;
        _member = member;
        _data = data;
        _tracker = metaData.CreateTracker();
    }
    
    public IPayloadProvider OnTurnStart()
    {
        if (!IsActive) 
            return new NoPayload();

        _tracker.AdvanceTurn();
        return _data.Play(new StatusEffectContext(_member));
    }

    public IPayloadProvider OnTurnEnd() => new NoPayload();
    public ITemporalState CloneOriginal() => new AtStartOfTurn(_ctx, _member, _data, _tracker.Metadata);
}
