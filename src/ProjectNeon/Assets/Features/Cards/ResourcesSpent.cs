public class ResourcesSpent
{
    public int Amount;
    public ResourceType ResourceType;

    public override string ToString() => $"{Amount} {ResourceType.Name}";
}