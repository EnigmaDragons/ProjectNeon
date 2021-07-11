
public class CorpGearShopData
{
    public string ShopName { get; }
    public CorpAffinityLines AffinityLines { get; }

    public CorpGearShopData(string shopName, CorpAffinityLines affinityLines)
    {
        ShopName = shopName;
        AffinityLines = affinityLines;
    }
}