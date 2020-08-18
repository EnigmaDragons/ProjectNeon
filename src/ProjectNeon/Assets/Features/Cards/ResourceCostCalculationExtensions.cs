
public static class ResourceCostCalculationExtensions
{
    public static ResourceQuantity ResourcesSpent(this ResourceCost cost, Member m)
    {
        if (cost == null || cost.ResourceType == null)
            return new ResourceQuantity {Amount = 0, ResourceType = ""};
    
        return new ResourceQuantity
        {
            ResourceType = cost.ResourceType.Name,
            Amount = cost.IsXCost
                ? m.State[cost.ResourceType]
                : cost.Amount
        };
    }
    
    public static ResourceQuantity ResourcesGained(this ResourceCost gain, Member m)
    {
        if (gain == null || gain.ResourceType == null)
            return new ResourceQuantity {Amount = 0, ResourceType = ""};
        
        return new ResourceQuantity
        {
            ResourceType = gain.ResourceType.Name,
            Amount = gain.IsXCost
                ? m.ResourceMax(gain.ResourceType)
                : gain.Amount
        };
    }
}
