using System;
using UnityEngine;

[Serializable]
public sealed class InMemoryResourceType : IResourceType
{
    public string Name { get; set; } = "None";
    public int MaxAmount { get; set; } = 0;
    public int StartingAmount { get; set; } = 0;

    public InMemoryResourceType() : this("None") {}
    public InMemoryResourceType(string name) => Name = name;
    public InMemoryResourceType(IResourceType r)
    {
        Name = r.Name;
        MaxAmount = r.MaxAmount;
        StartingAmount = r.StartingAmount;
    }
    public InMemoryResourceType(IResourceType first, IResourceType second, Func<int, int, int> operation)
    {
        if (!first.Name.Equals(second.Name))
            throw new ArgumentException("Cannot add {first.Name} to a {second.Name} modifier");
        Name = first.Name;
        MaxAmount = operation(first.MaxAmount, second.MaxAmount);
        StartingAmount = operation(first.StartingAmount, second.StartingAmount);
    }
}

public static class ResourceTypeExtensions
{
    public static string GetLocalizedName(this IResourceType r) => r.GetTerm().ToLocalized();
    public static string GetTerm(this IResourceType r) => $"Resources/{r.Name}";
    
    public static InMemoryResourceType WithMax(this IResourceType r, int max) => r.WithAmounts(r.StartingAmount, max); 
    
    public static InMemoryResourceType WithAmounts(this IResourceType r, int starting) => new InMemoryResourceType
    {
        Name = r.Name,
        StartingAmount =  starting,
        MaxAmount = r.MaxAmount
    };
    
    public static InMemoryResourceType WithAmounts(this IResourceType r, int starting, int max) => new InMemoryResourceType
    {
        Name = r.Name,
        StartingAmount =  starting,
        MaxAmount = max
    };
    
    public static InMemoryResourceType WithName(this IResourceType r, string name) => new InMemoryResourceType
    {
        Name = name,
        StartingAmount =  r.StartingAmount,
        MaxAmount = r.MaxAmount
    };

    public static IResourceType WithPrimaryResourceMappedForOwner(this IResourceType r, IResourceType primaryResourceType) 
        => r.Name.Equals("PrimaryResource") 
            ? r.WithName(primaryResourceType.Name) 
            : r;
}
