
public class ShowCards
{
    public string CardZoneNameTerm { get; }
    public Card[] Cards { get; }

    public ShowCards(string cardZoneNameTerm, Card[] cards)
    {
        CardZoneNameTerm = cardZoneNameTerm;
        Cards = cards;
    }
}
