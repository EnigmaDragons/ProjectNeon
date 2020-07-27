public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourceQuantity _spent;
    private readonly ResourceQuantity _gained;

    public PlayedCardV2(Member performer, Target[] targets, Card card)
        : this(performer, targets, card, card.Type.ResourcesSpent(performer), card.Type.ResourcesGained(performer)) {}
    public PlayedCardV2(Member performer, Target[] targets, Card card, ResourceQuantity spent, ResourceQuantity gained)
    {
        _performer = performer;
        _targets = targets;
        _card = card;
        _spent = spent;
        _gained = gained;
    }

    public Member Member => _performer;
    public Card Card => _card;
    public ResourceQuantity Spent => _spent;
    public ResourceQuantity Gained => _gained;

    public void Perform()
    {
        Message.Subscribe<SequenceFinished>(_ =>
        {
            Message.Unsubscribe(this);
            Message.Publish(new CardResolutionFinished());
        }, this);
        Card.Play(_performer, _targets, _spent.Amount);
    }
}
