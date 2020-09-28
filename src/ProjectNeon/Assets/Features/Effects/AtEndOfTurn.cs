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
        ctx.Target.ApplyToAllConscious(m => m.ApplyTemporaryAdditive(
            new AtEndOfTurn(ctx, _source.ReferencedSequence, 
                TemporalStateMetadata.ForDuration(_source.NumberOfTurns, _source.EffectScope.Value.Equals("Debuff")))));
    }
}

public class AtEndOfTurn : ITemporalState
{
    private readonly TemporalStateTracker _tracker;
    private readonly EffectContext _ctx;
    private readonly CardActionsData _data;

    public IStats Stats { get; } = new StatAddends();
    public StatusTag Tag => StatusTag.None;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;

    public AtEndOfTurn(EffectContext ctx, CardActionsData data, TemporalStateMetadata metadata)
    {
        _tracker = metadata.CreateTracker();
        _ctx = ctx;
        _data = data;
    }
    
    public void OnTurnStart() {}

    public void OnTurnEnd()
    {
        if (!IsActive) return;

        BattleLog.Write($"Applying End of Turn effect. {_data.name}");
        _tracker.AdvanceTurn();
        foreach (var action in _data.Actions)
            if (action.Type == CardBattleActionType.Battle)
                AllEffects.Apply(action.BattleEffect, _ctx);
            // TODO: Needs to be able to resolve PayloadProviders
    }

    public ITemporalState CloneOriginal() => new AtEndOfTurn(_ctx, _data, _tracker.Metadata);
}