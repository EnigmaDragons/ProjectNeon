using System;
using System.Collections.Generic;

public abstract class EffectTransformerBase : EffectTransformer
{
    private readonly int _maxDurationTurns;
    private readonly int _maxUses;
    private readonly Func<EffectData, EffectContext, bool> _shouldModify;
    private readonly Func<EffectData, EffectContext, EffectData> _modify;
    
    private int _remainingDurationTurns;
    private int _remainingUses;
    private bool HasMoreUses => _remainingUses != 0;
    private bool HasMoreTurns => _remainingDurationTurns != 0;
    private HashSet<int> _cardIds = new HashSet<int>();
    
    public IStats Stats => new StatAddends();
    public bool IsDebuff { get; }
    public bool IsActive => HasMoreUses && HasMoreTurns;
    public Maybe<int> Amount => _remainingUses;
    public Maybe<int> RemainingTurns => _remainingDurationTurns;

    public EffectTransformerBase(bool isDebuff, int maxDurationTurns, int maxUses, Func<EffectData, EffectContext, bool> shouldModify, Func<EffectData, EffectContext, EffectData> modify)
    {
        _maxDurationTurns = maxDurationTurns;
        _maxUses = maxUses;
        _remainingDurationTurns = maxDurationTurns;
        _remainingUses = maxUses;
        _shouldModify = shouldModify;
        _modify = modify;
        IsDebuff = isDebuff;
        if (!IsActive)
            Log.Error($"{GetType()} was created inactive with {maxUses} Uses and {maxDurationTurns} Turns");
    }

    public ITemporalState CloneOriginal()
        => new ClonedEffectTransformer(Tag, IsDebuff, _maxDurationTurns, _maxUses, _shouldModify, _modify);

    public IPayloadProvider OnTurnStart() => new NoPayload();

    public IPayloadProvider OnTurnEnd()
    {
        if (_remainingDurationTurns > 0)
            _remainingDurationTurns--;
        return new NoPayload();
    }

    public EffectData Modify(EffectData effect, EffectContext context)
    {
        if (!_shouldModify(effect, context) 
            || context.Card.IsMissing 
            || (!_cardIds.Contains(context.Card.Value.Id) && _remainingUses == 0))
            return effect;
        if (!_cardIds.Contains(context.Card.Value.Id))
        {
            _cardIds.Add(context.Card.Value.Id);
            if (_remainingUses > 0)
                _remainingUses--;
        }
        return _modify(effect, context);
    }

    public abstract StatusTag Tag { get; }
}

public class ClonedEffectTransformer : EffectTransformerBase
{
    public ClonedEffectTransformer(StatusTag tag, bool isDebuff, int maxDurationTurns, int maxUses, Func<EffectData, EffectContext, bool> shouldModify, Func<EffectData, EffectContext, EffectData> modify) 
        : base(isDebuff, maxDurationTurns, maxUses, shouldModify, modify)
    {
        Tag = tag;
    }

    public override StatusTag Tag { get; }
}