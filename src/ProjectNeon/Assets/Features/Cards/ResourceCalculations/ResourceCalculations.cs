public class ResourceCalculations
{
    public static readonly ResourceCalculations Free = new ResourceCalculations(new InMemoryResourceType(), 0, 0, 0);
    
    public IResourceType ResourcePaidType { get; }
    public int ResourcesPaid { get; }
    public int XAmount { get; }
    public int XAmountPriceTag { get; }

    public ResourceCalculations(IResourceType resourcePaidType, int resourcesPaid, int xAmount, int xAmountPriceTag)
    {
        ResourcePaidType = resourcePaidType;
        ResourcesPaid = resourcesPaid;
        XAmount = xAmount;
        XAmountPriceTag = xAmountPriceTag;
    }
    
    public ResourceQuantity PaidQuantity => new ResourceQuantity { Amount = ResourcesPaid, ResourceType = ResourcePaidType.Name };
    public ResourceQuantity XAmountQuantity => new ResourceQuantity { Amount = XAmount, ResourceType = ResourcePaidType.Name };
}