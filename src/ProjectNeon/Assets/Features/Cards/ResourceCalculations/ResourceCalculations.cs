public class ResourceCalculations
{
    public static readonly ResourceCalculations Free = new ResourceCalculations(new InMemoryResourceType(), 0, new InMemoryResourceType(), 0, 0, 0);
    
    public IResourceType ResourcePaidType { get; }
    public int ResourcesPaid { get; }
    public IResourceType ResourceGainedType { get; }
    public int ResourcesGained { get; }
    public int XAmount { get; }
    public int XAmountPriceTag { get; }

    public ResourceCalculations(IResourceType resourcePaidType, int resourcesPaid, IResourceType resourceGainedType, int resourcesGained, int xAmount, int xAmountPriceTag)
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