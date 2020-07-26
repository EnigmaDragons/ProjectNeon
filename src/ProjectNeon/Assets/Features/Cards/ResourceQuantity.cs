public class ResourceQuantity
{
    public int Amount;
    public IResourceType ResourceType;

    public override string ToString() => $"{Amount} {ResourceType.Name}";
}
