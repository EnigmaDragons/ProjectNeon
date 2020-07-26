public class PlayedCardV2 : IPlayedCard
{
    private readonly Card _card;
    private readonly Member _performer;
    private readonly Target[] _targets;
    private readonly ResourcesSpent _spent;

    public PlayedCardV2(Member performer, Target[] targets, Card card, ResourcesSpent spent)
    {
        _performer = performer;
        _targets = targets;
        _card = card;
        _spent = spent;
    }

    public Member Member => _performer;
    public Card Card => _card;
    public ResourcesSpent Spent => _spent;

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