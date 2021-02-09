
public interface IResourceAmount
{
    int BaseAmount { get; }
    IResourceType ResourceType { get; }
    bool PlusXCost { get; }
}
