
public class StartOfTurnEffect : Effect
{
    private readonly EffectData _source;

    public StartOfTurnEffect(EffectData source)
    {
        _source = source;
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m => m.ApplyTemporaryAdditive(
            new AtStartOfTurn(ctx, _source.ReferencedSequence, 
                TemporalStateMetadata.ForDuration(_source.NumberOfTurns, _source.EffectScope.Value.Equals("Debuff")))));
    }
}

public class AtStartOfTurn : ITemporalState
{
    private readonly TemporalStateTracker _tracker;
    private readonly EffectContext _ctx;
    private readonly CardActionsData _data;

    public IStats Stats { get; } = new StatAddends();
    public StatusTag Tag => StatusTag.None;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;

    public AtStartOfTurn(EffectContext ctx, CardActionsData data, TemporalStateMetadata metaData)
    {
        _ctx = ctx;
        _data = data;
        _tracker = metaData.CreateTracker();
    }
    
    public void OnTurnStart()
    {
        if (!IsActive) return;

        _tracker.AdvanceTurn();
        foreach(var action in _data.Actions)
            if (action.Type == CardBattleActionType.Battle)
                AllEffects.Apply(action.BattleEffect, _ctx);
    }

    public void OnTurnEnd() {}
    public ITemporalState CloneOriginal() => new AtStartOfTurn(_ctx, _data, _tracker.Metadata);
}
