
public class ShowCards
{
    public string CardZoneName { get; }
    public Card[] Cards { get; }

    public ShowCards(string cardZoneName, Card[] cards)
    {
        CardZoneName = cardZoneName;
        Cards = cards;
    }
}
