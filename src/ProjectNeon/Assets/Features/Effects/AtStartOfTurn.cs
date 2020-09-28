using System;

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
            new AtStartOfTurn(ctx, _source.ReferencedSequence, _source.NumberOfTurns, _source.EffectScope.Value.Equals("Debuff"))));
    }
}

public class AtStartOfTurn : ITemporalState
{
    private readonly int _originalTurns;
    private readonly bool _indefinite;
    private readonly EffectContext _ctx;
    private readonly CardActionsData _data;
    private readonly Member _member;
    private int _remainingDuration;

    public IStats Stats { get; } = new StatAddends();
    public StatusTag Tag => StatusTag.None;
    public bool IsDebuff { get; }
    public bool IsActive => _remainingDuration > 0 || _indefinite;

    public AtStartOfTurn(EffectContext ctx, CardActionsData data, int turns, bool isDebuff)
    {
        IsDebuff = isDebuff;
        _indefinite = turns < 0;
        _ctx = ctx;
        _data = data;
        _remainingDuration = turns;
        _originalTurns = turns;
    }
    
    public void OnTurnStart()
    {
        if (!IsActive) return;

        _remainingDuration--;
        foreach(var action in _data.Actions)
            if (action.Type == CardBattleActionType.Battle)
                AllEffects.Apply(action.BattleEffect, _ctx);
    }

    public void OnTurnEnd() {}
    public ITemporalState CloneOriginal() => new AtStartOfTurn(_ctx, _data, _originalTurns, IsDebuff);
}
