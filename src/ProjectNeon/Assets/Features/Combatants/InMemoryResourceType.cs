using UnityEngine;

public sealed class InMemoryResourceType : IResourceType
{
    public string Name { get; set; } = "None";
    public Sprite Icon { get; set; } = null;
    public int MaxAmount { get; set; } = 0;
    public int StartingAmount { get; set; } = 0;

    public InMemoryResourceType() : this("None") {}
    public InMemoryResourceType(string name) => Name = name;
}

public static class ResourceTypeExtensions
{
    public static IResourceType WithMax(this IResourceType r, int max) => r.WithAmounts(r.StartingAmount, max); 
    
    public static IResourceType WithAmounts(this IResourceType r, int starting) => new InMemoryResourceType
    {
        Name = r.Name,
        Icon = r.Icon,
        StartingAmount =  starting,
        MaxAmount = r.MaxAmount
    };
    
    public static IResourceType WithAmounts(this IResourceType r, int starting, int max) => new InMemoryResourceType
    {
        Name = r.Name,
        Icon = r.Icon,
        StartingAmount =  starting,
        MaxAmount = max
    };
    
    public static IResourceType WithName(this IResourceType r, string name) => new InMemoryResourceType
    {
        Name = name,
        Icon = r.Icon,
        StartingAmount =  r.StartingAmount,
        MaxAmount = r.MaxAmount
    };

    public static IResourceType WithPrimaryResourceMappedForOwner(this IResourceType r, IResourceType primaryResourceType) 
        => r.Name.Equals("PrimaryResource") 
            ? r.WithName(primaryResourceType.Name) 
            : r;
}
