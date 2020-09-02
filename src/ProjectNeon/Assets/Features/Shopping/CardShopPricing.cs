public static class CardPricing
{
    public static int ShopPrice(this CardType c)
    {
        if (c.Rarity == Rarity.Common)
            return WithShopPricingVariance(50);
        if (c.Rarity == Rarity.Uncommon)
            return WithShopPricingVariance(75);
        if (c.Rarity == Rarity.Rare)
            return WithShopPricingVariance(125);
        if (c.Rarity == Rarity.Epic)
            return WithShopPricingVariance(175);
        return WithShopPricingVariance(25);
    }

    public static int WithShopPricingVariance(this int target) 
        => Rng.Int((target * 0.8f).FlooredInt(), (target * 1.2f).CeilingInt());
}
