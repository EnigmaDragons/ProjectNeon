public class ResourceCalculations
{
    public ResourceType ResourcePaidType { get; }
    public int ResourcesPaid { get; }
    public ResourceType ResourceGainedType { get; }
    public int ResourcesGained { get; }
    public int XAmount { get; }
    public int XAmountPriceTag { get; }

    public ResourceCalculations(ResourceType resourcePaidType, int resourcesPaid, ResourceType resourceGainedType, int resourcesGained, int xAmount, int xAmountPriceTag)
    {
        ResourcePaidType = resourcePaidType;
        ResourcesPaid = resourcesPaid;
        ResourceGainedType = resourceGainedType;
        ResourcesGained = resourcesGained;
        XAmount = xAmount;
        XAmountPriceTag = xAmountPriceTag;
    }
    
    public ResourceQuantity PaidQuantity => new ResourceQuantity { Amount = ResourcesPaid, ResourceType = ResourcePaidType.Name };
    public ResourceQuantity GainedQuantity => new ResourceQuantity { Amount = ResourcesGained, ResourceType = ResourceGainedType.Name };
    public ResourceQuantity XAmountQuantity => new ResourceQuantity { Amount = XAmount, ResourceType = ResourcePaidType.Name };
}