using System.IO;

public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourceCalculations _calculations;
    private readonly bool _isTransient;

    public PlayedCardV2(Member performer, Target[] targets, Card card, bool isTransient = false)
        : this(performer, targets, card, isTransient, performer.CalculateResources(card.Type)) {}
    
    public PlayedCardV2(Member performer, Target[] targets, Card card, bool isTransient, ResourceCalculations calculations)
    {
        if (targets.Length < card.ActionSequences.Length)
            throw new InvalidDataException($"Cannot play {card.Name} with only {targets.Length}");
        
        _performer = performer;
        _targets = targets;
        _card = card;
        _calculations = calculations;
        _isTransient = isTransient;
        _card.SetXValue(new ResourceQuantity { Amount = calculations.XAmount, ResourceType = calculations.ResourcePaidType.Name });
    }

    public Member Member => _performer;
    public Card Card => _card;
    public Target[] Targets => _targets;
    public ResourceQuantity Spent => _calculations.PaidQuantity;
    public ResourceQuantity Gained => _calculations.GainedQuantity;
    public bool IsTransient => _isTransient;

    public void Perform(BattleStateSnapshot beforeCard)
    {
        var wasInstant = this.IsInstant();
        Message.Subscribe<MessageGroupFinished>(_ =>
        {
            Message.Unsubscribe(this);
            Message.Publish(new CardResolutionFinished(Member.Id, wasInstant));
        }, this);
        Card.Play(_targets, beforeCard);
    }
}
