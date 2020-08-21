public static class CardPricing
{
    public static int ShopPrice(this CardType c)
    {
        if (c.Rarity == Rarity.Common)
            return 25;
        if (c.Rarity == Rarity.Uncommon)
            return 50;
        if (c.Rarity == Rarity.Rare)
            return 75;
        if (c.Rarity == Rarity.Epic)
            return 125;
        return 25;
    }
}
