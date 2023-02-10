
public class BonusCardDetails
{
    public CardTypeData Card { get; }
    public ResourceQuantity Cost { get; }

    public BonusCardDetails(CardTypeData card, ResourceQuantity cost)
    {
        Card = card;
        Cost = cost;
    }
}
