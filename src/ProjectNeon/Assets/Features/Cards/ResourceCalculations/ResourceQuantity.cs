public class ResourceQuantity
{
    public static readonly ResourceQuantity DontInterpolateX = new ResourceQuantity { Amount = -1, ResourceType = "None" };
    public static readonly ResourceQuantity None = new ResourceQuantity { Amount = 0, ResourceType = "None" };
    
    public int Amount;
    public string ResourceType;

    public override string ToString() => $"{Amount} {ResourceType}";
}
