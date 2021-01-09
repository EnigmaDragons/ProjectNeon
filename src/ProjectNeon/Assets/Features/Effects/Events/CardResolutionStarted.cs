public sealed class CardResolutionStarted
{
    public IPlayedCard Card { get; set; }
    public int Originator => Card.Member.Id;

    public CardResolutionStarted(IPlayedCard c) => Card = c;
}
