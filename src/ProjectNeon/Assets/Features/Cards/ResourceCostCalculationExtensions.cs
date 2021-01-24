
using System;

public static class ResourceCostCalculationExtensions
{
    public static ResourceQuantity XAmountSpent(this ResourceCost cost, Member m)
    {
        if (cost == null || cost.ResourceType == null)
            return ResourceQuantity.None;
    
        return new ResourceQuantity
        {
            ResourceType = cost.ResourceType.Name,
            Amount = Math.Max(0, cost.PlusXCost
                ? m.State[cost.ResourceType] - cost.BaseAmount
                : cost.BaseAmount)
        };
    }
    
    public static ResourceQuantity ResourcesSpent(this ResourceCost cost, Member m)
    {
        if (cost == null || cost.ResourceType == null)
            return ResourceQuantity.None;
    
        return new ResourceQuantity
        {
            ResourceType = cost.ResourceType.Name,
            Amount = cost.PlusXCost
                ? m.State[cost.ResourceType]
                : cost.BaseAmount
        };
    }
    
    public static ResourceQuantity ResourcesGained(this ResourceCost gain, Member m)
    {
        if (gain == null || gain.ResourceType == null)
            return ResourceQuantity.None;

        var resourceType = gain.ResourceType;
        var remainingResourceAmount = m.ResourceMax(resourceType) - m.ResourceAmount(resourceType);
        return new ResourceQuantity
        {
            ResourceType = resourceType.Name,
            Amount = gain.PlusXCost
                ? remainingResourceAmount
                : Math.Min(gain.BaseAmount, remainingResourceAmount)
        };
    }
}
