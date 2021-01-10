using System.IO;

public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourceQuantity _spent;
    private readonly ResourceQuantity _gained;
    private readonly ResourceQuantity _xAmountSpent;
    private readonly bool _isTransient;

    public PlayedCardV2(Member performer, Target[] targets, Card card, bool isTransient = false)
        : this(performer, targets, card, isTransient,
            card.Cost.ResourcesSpent(performer), card.Type.Gain.ResourcesGained(performer), card.Type.Cost.XAmountSpent(performer)) {}
    
    public PlayedCardV2(Member performer, Target[] targets, Card card, bool isTransient, ResourceQuantity spent, ResourceQuantity gained, ResourceQuantity xAmountSpent)
    {
        if (targets.Length < card.ActionSequences.Length)
            throw new InvalidDataException($"Cannot play {card.Name} with only {targets.Length}");
        
        _performer = performer;
        _targets = targets;
        _card = card;
        _spent = spent;
        _gained = gained;
        _xAmountSpent = xAmountSpent;
        _isTransient = isTransient;
    }

    public Member Member => _performer;
    public Card Card => _card;
    public Target[] Targets => _targets;
    public ResourceQuantity Spent => _spent;
    public ResourceQuantity Gained => _gained;
    public bool IsTransient => _isTransient;

    public void Perform(BattleStateSnapshot beforeCard)
    {
        Message.Subscribe<SequenceFinished>(_ =>
        {
            Message.Unsubscribe(this);
            Message.Publish(new CardResolutionFinished(Member.Id));
        }, this);
        Card.Play(_targets, _xAmountSpent, beforeCard);
    }
}
