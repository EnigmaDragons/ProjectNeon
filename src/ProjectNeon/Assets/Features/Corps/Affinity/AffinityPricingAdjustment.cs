
public static class AffinityPricingAdjustment
{
    public static float PriceFactor(PartyCorpAffinity partyAffinity, string corpName)
    {
        var corpAffinity = partyAffinity[corpName];
        return 1f + (-(int)corpAffinity) * 0.05f;
    }
}