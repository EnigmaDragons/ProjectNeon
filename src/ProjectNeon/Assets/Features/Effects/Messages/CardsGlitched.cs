
public class CardsGlitched
{
    public CardGlitchedDetails[] Cards { get; }

    public CardsGlitched(CardGlitchedDetails[] cards) => Cards = cards;
}

public class CardGlitchedDetails
{
    public Card Card { get; }
    public CardLocation Location { get; }

    public CardGlitchedDetails(Card card, CardLocation loc)
    {
        Card = card;
        Location = loc;
    }
}
