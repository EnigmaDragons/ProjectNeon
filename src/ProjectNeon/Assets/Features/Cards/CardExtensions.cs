
public static class CardExtensions
{
    public static void ShowDetailedCardView(this Card c) => Message.Publish(new ShowDetailedCardView(c));
}
