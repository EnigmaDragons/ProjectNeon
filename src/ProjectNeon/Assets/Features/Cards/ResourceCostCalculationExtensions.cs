
public static class ResourceCostCalculationExtensions
{
    public static ResourcesSpent ResourcesSpent(this Card card, Member m)
    {
        var cost = card.Cost;
        return new ResourcesSpent
        {
            ResourceType = card.Cost.ResourceType,
            Amount = cost.IsXCost
                ? m.State[cost.ResourceType]
                : cost.Cost
        };
    }
}
