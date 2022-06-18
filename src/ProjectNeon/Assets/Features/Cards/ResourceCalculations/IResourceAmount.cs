
public interface IResourceAmount
{
    int BaseAmount { get; }
    IResourceType ResourceType { get; }
    bool PlusXCost { get; }
}

public static class ResourceAmountExtensions
{
    public static int CostSortOrder(this IResourceAmount cost) => cost.PlusXCost ? 99 : cost.BaseAmount;
}
