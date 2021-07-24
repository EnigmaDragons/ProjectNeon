using System;

public static class CardShopPricing
{
    public static int CardShopPrice(this CardTypeData c)
        => CardShopPrice(c.Rarity);
    public static int CardShopPrice(this Rarity rarity)
    {
        if (rarity == Rarity.Common)
            return 40;
        if (rarity == Rarity.Uncommon)
            return 70;
        if (rarity == Rarity.Rare)
            return 140;
        if (rarity == Rarity.Epic)
            return 250;
        return 25;
    }

    public static int EquipmentShopPrice(this Rarity rarity, float priceFactor)
    {
        if (rarity == Rarity.Common)
            return Int(80 * priceFactor);
        if (rarity == Rarity.Uncommon)
            return Int(180 * priceFactor);
        if (rarity == Rarity.Rare)
            return Int(280 * priceFactor);
        if (rarity == Rarity.Epic)
            return Int(400 * priceFactor);
        return 50;
    }

    public static int WithShopPricingVariance(this int target)
        => WithShopPricingVariance(Convert.ToSingle(target));
    public static int WithShopPricingVariance(this float target) 
        => Rng.Int((target * 0.8f).FlooredInt(), (target * 1.2f).CeilingInt());
    public static int Int(float f) => f.CeilingInt();
}
