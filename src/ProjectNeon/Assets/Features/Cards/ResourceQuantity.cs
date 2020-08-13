public class ResourceQuantity
{
    public int Amount;
    public string ResourceType;

    public override string ToString() => $"{Amount} {ResourceType}";
}
