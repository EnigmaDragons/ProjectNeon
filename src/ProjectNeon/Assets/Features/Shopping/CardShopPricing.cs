public static class CardPricing
{
    public static int ShopPrice(this CardType c)
    {
        if (c.Rarity == Rarity.Common)
            return WithVariance(25);
        if (c.Rarity == Rarity.Uncommon)
            return WithVariance(50);
        if (c.Rarity == Rarity.Rare)
            return WithVariance(75);
        if (c.Rarity == Rarity.Epic)
            return WithVariance(125);
        return WithVariance(25);
    }

    private static int WithVariance(int target) => Rng.Int((target * 0.8f).FlooredInt(), (target * 1.2f).CeilingInt());
}
