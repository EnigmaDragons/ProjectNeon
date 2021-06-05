using System;
using System.Collections.Generic;

public abstract class EffectTransformerBase : EffectTransformer
{
    private TemporalStateTracker _tracker;
    private readonly Func<EffectData, EffectContext, bool> _shouldModify;
    private readonly Func<EffectData, EffectContext, EffectData> _modify;
    private HashSet<int> _cardIds = new HashSet<int>();

    public int OriginatorId => _tracker.Metadata.OriginatorId;
    public Maybe<int> Amount => _tracker.RemainingUses;
    public IStats Stats => new StatAddends();
    public StatusDetail Status => _tracker.Status;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;
    public Maybe<int> RemainingTurns => _tracker.RemainingTurns;
    
    public EffectTransformerBase(int originatorId, bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, Func<EffectData, EffectContext, bool> shouldModify, Func<EffectData, EffectContext, EffectData> modify)
        : this(new TemporalStateMetadata(originatorId, isDebuff, maxUses, maxDurationTurns, status), shouldModify, modify) {}
    public EffectTransformerBase(TemporalStateMetadata metadata, Func<EffectData, EffectContext, bool> shouldModify, Func<EffectData, EffectContext, EffectData> modify)
    {
        _tracker = new TemporalStateTracker(metadata);
        _shouldModify = shouldModify;
        _modify = modify;
    }

    public ITemporalState CloneOriginal()
        => new ClonedEffectTransformer(_tracker.Metadata, _shouldModify, _modify);

    public IPayloadProvider OnTurnStart() => new NoPayload();

    public IPayloadProvider OnTurnEnd()
    {
        _tracker.AdvanceTurn();
        _cardIds.Clear();
        return new NoPayload();
    }

    public EffectData Modify(EffectData effect, EffectContext context)
    {
        if (!_shouldModify(effect, context) 
            || context.Card.IsMissing 
            || (!_cardIds.Contains(context.Card.Value.Id) && !_tracker.IsActive))
            return effect;
        if (!_cardIds.Contains(context.Card.Value.Id))
        {
            _cardIds.Add(context.Card.Value.Id);
            _tracker.RecordUse();
        }
        return _modify(effect, context);
    }
}

public class ClonedEffectTransformer : EffectTransformerBase
{
    public ClonedEffectTransformer(TemporalStateMetadata metadata, Func<EffectData, EffectContext, bool> shouldModify, Func<EffectData, EffectContext, EffectData> modify) 
        : base(metadata, shouldModify, modify) {}
}