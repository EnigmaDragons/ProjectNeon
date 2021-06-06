
public class InMemoryResourceAmount : IResourceAmount
{
    public int BaseAmount { get; }
    public IResourceType ResourceType { get; }
    public bool PlusXCost { get; }

    public InMemoryResourceAmount(int amount, string resourceType = "None", bool plusXCost = false)
        : this(amount, new InMemoryResourceType(resourceType), plusXCost) {}

    public InMemoryResourceAmount(int amount, IResourceType type, bool plusXCost)
    {
        BaseAmount = amount;
        ResourceType = type;
        PlusXCost = plusXCost;
    }
}
