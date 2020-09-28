using UnityEngine;

public class EndOfTurnEffect : Effect
{
    private readonly EffectData _source;

    public EndOfTurnEffect(EffectData source)
    {
        _source = source;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConsciousMembers(m => m.State.ApplyTemporaryAdditive(
            new AtEndOfTurn(ctx, m, _source.ReferencedSequence, 
                TemporalStateMetadata.ForDuration(_source.NumberOfTurns, _source.EffectScope.Value.Equals("Debuff")))));
    }
}

public class AtEndOfTurn : ITemporalState
{
    private readonly TemporalStateTracker _tracker;
    private readonly EffectContext _ctx;
    private readonly Member _member;
    private readonly CardActionsData _data;

    public IStats Stats { get; } = new StatAddends();
    public StatusTag Tag => StatusTag.None;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;

    public AtEndOfTurn(EffectContext ctx, Member member, CardActionsData data, TemporalStateMetadata metadata)
    {
        _tracker = metadata.CreateTracker();
        _ctx = ctx;
        _member = member;
        _data = data;
    }
    
    public IPayloadProvider OnTurnStart() => new NoPayload();

    public IPayloadProvider OnTurnEnd()
    {
        if (!IsActive) 
            return new NoPayload();

        _tracker.AdvanceTurn();
        return _data.Play(new StatusEffectContext(_member));
    }

    public ITemporalState CloneOriginal() => new AtEndOfTurn(_ctx, _member, _data, _tracker.Metadata);
}