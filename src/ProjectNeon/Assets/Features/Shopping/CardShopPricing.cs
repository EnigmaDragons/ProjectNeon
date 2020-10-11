public static class CardPricing
{
    public static int ShopPrice(this CardType c)
    {
        if (c.Rarity == Rarity.Common)
            return WithShopPricingVariance(40);
        if (c.Rarity == Rarity.Uncommon)
            return WithShopPricingVariance(80);
        if (c.Rarity == Rarity.Rare)
            return WithShopPricingVariance(160);
        if (c.Rarity == Rarity.Epic)
            return WithShopPricingVariance(320);
        return WithShopPricingVariance(25);
    }

    public static int WithShopPricingVariance(this int target) 
        => Rng.Int((target * 0.8f).FlooredInt(), (target * 1.2f).CeilingInt());
}
