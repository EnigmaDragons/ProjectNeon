using System;

public static class CardShopPricing
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

    public static int EquipmentShopPrice(Rarity rarity, float priceFactor)
    {
        if (rarity == Rarity.Common)
            return WithShopPricingVariance(100 * priceFactor);
        if (rarity == Rarity.Uncommon)
            return WithShopPricingVariance(200 * priceFactor);
        if (rarity == Rarity.Rare)
            return WithShopPricingVariance(400 * priceFactor);
        if (rarity == Rarity.Epic)
            return WithShopPricingVariance(600 * priceFactor);
        return WithShopPricingVariance(50);
    }

    public static int WithShopPricingVariance(this int target)
        => WithShopPricingVariance(Convert.ToSingle(target));
    public static int WithShopPricingVariance(this float target) 
        => Rng.Int((target * 0.8f).FlooredInt(), (target * 1.2f).CeilingInt());
}
