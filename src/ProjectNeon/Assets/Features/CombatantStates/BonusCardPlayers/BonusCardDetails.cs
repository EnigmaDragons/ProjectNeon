
public class BonusCardDetails
{
    public CardType Card { get; }
    public ResourceQuantity Cost { get; }

    public BonusCardDetails(CardType card, ResourceQuantity cost)
    {
        Card = card;
        Cost = cost;
    }
}
