
public static class ResourceCostCalculationExtensions
{
    public static ResourceQuantity ResourcesSpent(this Card card, Member m)
    {
        var cost = card.Cost;
        return new ResourceQuantity
        {
            ResourceType = cost.ResourceType,
            Amount = cost.IsXCost
                ? m.State[cost.ResourceType]
                : cost.Amount
        };
    }
    
    public static ResourceQuantity ResourcesGained(this Card card, Member m)
    {
        if (card.Gain == null || card.Gain.ResourceType == null)
            return new ResourceQuantity {Amount = 0, ResourceType = new InMemoryResourceType()};
        
        var gain = card.Gain;
        return new ResourceQuantity
        {
            ResourceType = gain.ResourceType,
            Amount = gain.IsXCost
                ? m.ResourceMax(gain.ResourceType)
                : gain.Amount
        };
    }
}
