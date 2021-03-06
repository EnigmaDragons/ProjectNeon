﻿using System.IO;

public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourceCalculations _calculations;
    private readonly bool _isSingleUse;
    private readonly bool _isTransient;
    private readonly ResourceQuantity _lockedXValue;

    public PlayedCardV2(Member performer, Target[] targets, Card card)
        : this(performer, targets, card, false, performer.CalculateResources(card.Type)) {}
    
    public PlayedCardV2(Member performer, Target[] targets, Card card, bool isTransient)
        : this(performer, targets, card, isTransient, performer.CalculateResources(card.Type)) {}
    
    public PlayedCardV2(Member performer, Target[] targets, Card card, bool isTransient, ResourceCalculations calculations)
    {
        if (card.IsActive && targets.Length < card.ActionSequences.Length)
            throw new InvalidDataException($"Cannot play {card.Name} with only {targets.Length}");
        
        _performer = performer;
        _targets = targets;
        _card = card;
        _calculations = calculations;
        _isSingleUse = card.IsSinglePlay;
        _isTransient = isTransient;
        _card.SetXValue(new ResourceQuantity { Amount = calculations.XAmount, ResourceType = calculations.ResourcePaidType.Name });
        _lockedXValue = _card.LockedXValue.Value;
    }

    public Member Member => _performer;
    public Card Card => _card;
    public Target[] Targets => _targets;
    public ResourceQuantity Spent => _calculations.PaidQuantity;
    public ResourceQuantity Gained => _calculations.GainedQuantity;
    public bool IsSingleUse => _isSingleUse;
    public bool IsTransient => _isTransient;

    public void Perform(BattleStateSnapshot beforeCard)
    {
        Card.Play(_targets, beforeCard, _lockedXValue, () => Message.Publish(new CardResolutionFinished(Member.Id)));
    }
}
