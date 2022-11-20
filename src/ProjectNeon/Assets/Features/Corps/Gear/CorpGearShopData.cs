
public class CorpGearShopData
{
    public string ShopNameTerm { get; }
    public CorpAffinityLines AffinityLines { get; }

    public CorpGearShopData(string shopNameTerm, CorpAffinityLines affinityLines)
    {
        ShopNameTerm = shopNameTerm;
        AffinityLines = affinityLines;
    }
}