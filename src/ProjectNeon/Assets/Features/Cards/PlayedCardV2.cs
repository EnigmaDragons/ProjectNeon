using System.IO;

public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourceQuantity _spent;
    private readonly ResourceQuantity _gained;

    public PlayedCardV2(Member performer, Target[] targets, Card card)
        : this(performer, targets, card, card.Cost.ResourcesSpent(performer), card.Type.Gain.ResourcesGained(performer)) {}
    public PlayedCardV2(Member performer, Target[] targets, Card card, ResourceQuantity spent, ResourceQuantity gained)
    {
        if (targets.Length < card.ActionSequences.Length)
            throw new InvalidDataException($"Cannot play {card.Name} with only {targets.Length}");
        
        _performer = performer;
        _targets = targets;
        _card = card;
        _spent = spent;
        _gained = gained;
    }

    public Member Member => _performer;
    public Card Card => _card;
    public Target[] Targets => _targets;
    public ResourceQuantity Spent => _spent;
    public ResourceQuantity Gained => _gained;

    public void Perform(BattleStateSnapshot beforeCard)
    {
        Message.Subscribe<SequenceFinished>(_ =>
        {
            Message.Unsubscribe(this);
            Message.Publish(new CardResolutionFinished());
        }, this);
        Card.Play(_targets, _spent, beforeCard);
    }
}
