using System;
using System.Collections.Generic;

public class ResourceCalculatorBase : ResourceCalculator
{
    private readonly TemporalStateTracker _tracker;
    private readonly Func<Card, bool> _isUsedOn;
    private readonly Func<CardTypeData, MemberState, ResourceCalculations> _calc;
    private readonly HashSet<int> _cardIds = new HashSet<int>();
    
    public Maybe<int> Amount => _tracker.RemainingUses;
    public IStats Stats => new StatAddends();
    public StatusDetail Status => _tracker.Status;
    public bool IsDebuff => _tracker.IsDebuff;
    public bool IsActive => _tracker.IsActive;
    public Maybe<int> RemainingTurns => _tracker.RemainingTurns;
    
    public ResourceCalculatorBase(bool isDebuff, int maxDurationTurns, int maxUses, StatusDetail status, Func<Card, bool> isUsedOn, Func<CardTypeData, MemberState, ResourceCalculations> calc)
        : this(new TemporalStateMetadata(isDebuff, maxUses, maxDurationTurns, status), isUsedOn, calc) {}
    public ResourceCalculatorBase(TemporalStateMetadata metadata, Func<Card, bool> isUsedOn, Func<CardTypeData, MemberState, ResourceCalculations> calc)
    {
        _tracker = new TemporalStateTracker(metadata);
        _isUsedOn = isUsedOn;
        _calc = calc;
    }

    public IPayloadProvider OnTurnStart() => new NoPayload();

    public IPayloadProvider OnTurnEnd()
    {
        _tracker.AdvanceTurn();
        _cardIds.Clear();
        return new NoPayload();
    }
    
    public void RecordUsageIfApplicable(Card card)
    {
        if (_isUsedOn(card))
            _cardIds.Add(card.Id);
        _tracker.RecordUse();
    }

    public void UndoUsageIfApplicable(Card card)
    {
        if (_cardIds.Contains(card.Id))
        {
            _cardIds.Remove(card.Id);
            _tracker.UndoUse();
        }
    }

    public ITemporalState CloneOriginal() => new ResourceCalculatorBase(_tracker.Metadata, _isUsedOn, _calc);
    
    public ResourceCalculations GetModifiers(CardTypeData card, MemberState member) => _calc(card, member);
}