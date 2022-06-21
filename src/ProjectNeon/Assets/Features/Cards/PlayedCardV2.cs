﻿using System.IO;

public class PlayedCardV2 : IPlayedCard
{
    private readonly int _playedCardId;
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourceCalculations _calculations;
    private readonly bool _isSingleUse;
    private readonly bool _isTransient;
    private readonly bool _retargetingAllowed;
    private readonly ResourceQuantity _lockedXValue;

    public PlayedCardV2(Member performer, Target[] targets, Card card, bool retargetingAllowed)
        : this(performer, targets, card, false, retargetingAllowed, performer.CalculateResources(card.Type)) {}
    
    public PlayedCardV2(Member performer, Target[] targets, Card card, bool isTransient, bool retargetingAllowed)
        : this(performer, targets, card, isTransient, retargetingAllowed, performer.CalculateResources(card.Type)) {}
    
    public PlayedCardV2(Member performer, Target[] targets, Card card, bool isTransient, bool retargetingAllowed, ResourceCalculations calculations)
    {
        if (card.IsActive && targets.Length < card.ActionSequences.Length)
            throw new InvalidDataException($"Cannot play {card.Name} with only {targets.Length}");

        _playedCardId = NextPlayedCardId.Get();
        _performer = performer;
        _targets = targets;
        _card = card;
        _calculations = calculations;
        _isSingleUse = card.IsSinglePlay;
        _isTransient = isTransient;
        _retargetingAllowed = retargetingAllowed;
        _card.SetXValue(new ResourceQuantity { Amount = calculations.XAmount, ResourceType = calculations.ResourcePaidType.Name });
        _lockedXValue = _card.LockedXValue.Value;
    }
    
    public int PlayedCardId => _playedCardId;
    public Member Member => _performer;
    public Card Card => _card;
    public Target[] Targets => _targets;
    public ResourceCalculations Calculations => _calculations; 
    public ResourceQuantity Spent => _calculations.PaidQuantity;
    public bool IsSingleUse => _isSingleUse;
    public bool IsTransient => _isTransient;
    public bool RetargetingAllowed => _retargetingAllowed;

    public void Perform(BattleStateSnapshot beforeCard)
    {
        Log.Info($"Perform {PlayedCardId}");
        Card.Play(_targets, beforeCard, _lockedXValue, () => Message.Publish(new CardResolutionFinished(this)));
    }

    public override string ToString() => $"Played - {Card.Name}";
}
