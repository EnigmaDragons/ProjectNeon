using System;

//not generic or flexible
public sealed class ReactWithCardAfterEnoughHealthLoss : ReactiveStateV2
{
    private readonly TemporalStateTracker _tracker;
    private readonly ReactionCardType _reaction;
    private readonly int _healthToTrigger;
    private readonly Member _possessor;
    
    private int _remainingHealthBeforeTrigger;

    public StatusDetail Status => _tracker.Status;
    public int OriginatorId => _tracker.Metadata.OriginatorId;
    public bool IsDebuff => false;
    public bool IsDot => false;
    public bool IsActive => _tracker.IsActive;
    public Maybe<int> RemainingTurns => _tracker.RemainingTurns;
    public Maybe<int> Amount => _tracker.RemainingUses;
    public IStats Stats => new StatAddends();
    public ReactionTimingWindow Timing => ReactionTimingWindow.ReactionCard;
    public bool OnlyReactDuringCardPhases => false;

    public ReactWithCardAfterEnoughHealthLoss(int originatorId, int maxUses, int duration, ReactionCardType reaction, int healthToTrigger, Member possessor)
    {
        _tracker = new TemporalStateTracker(new TemporalStateMetadata(originatorId, false, maxUses, duration, new StatusDetail(StatusTag.None)));
        _reaction = reaction;
        _healthToTrigger = healthToTrigger;
        _remainingHealthBeforeTrigger = healthToTrigger;
        _possessor = possessor;
    }
    
    public ProposedReaction[] OnEffectResolved(EffectResolved e)
    {
        if (!_tracker.IsActive || !e.WasApplied || !_possessor.IsConscious())
            return Array.Empty<ProposedReaction>();
        var healthLost = e.BattleBefore.Members[_possessor.Id].State.Hp - e.BattleAfter.Members[_possessor.Id].State.Hp;
        if (healthLost <= 0)
            return Array.Empty<ProposedReaction>();
        _remainingHealthBeforeTrigger -= healthLost;
        if (_remainingHealthBeforeTrigger > 0)
            return Array.Empty<ProposedReaction>();
        _remainingHealthBeforeTrigger = _healthToTrigger;
        _tracker.RecordUse();
        return new [] {new ProposedReaction(_reaction, _possessor, new Single(_possessor), ReactionTimingWindow.ReactionCard)};
    }

    public IPayloadProvider OnTurnStart() => new NoPayload();
    public IPayloadProvider OnTurnEnd()
    {
        _tracker.AdvanceTurn();
        return new NoPayload();
    }
    
    public ITemporalState CloneOriginal() => new ReactWithCardAfterEnoughHealthLoss(OriginatorId, _tracker.Metadata.MaxUses, _tracker.Metadata.MaxDurationTurns, _reaction, _healthToTrigger, _possessor);
}