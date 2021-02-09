
public class InMemoryResourceAmount : IResourceAmount
{
    public int BaseAmount { get; }
    public IResourceType ResourceType { get; }
    public bool PlusXCost { get; }

    public InMemoryResourceAmount(int amount, string resourceType = "None", bool plusXCost = false)
    {
        BaseAmount = amount;
        ResourceType = new InMemoryResourceType(resourceType);
        PlusXCost = plusXCost;
    }
}
