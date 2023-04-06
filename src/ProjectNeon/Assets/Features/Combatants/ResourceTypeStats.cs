using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceTypeStats
{
    private readonly Dictionary<string, InMemoryResourceType> _resources = new Dictionary<string, InMemoryResourceType>();

    public ResourceTypeStats WithAdded(params InMemoryResourceType[] resourceTypes) => With((first, second) => first + second, resourceTypes);
    public ResourceTypeStats WithSubtracted(params InMemoryResourceType[] resourceTypes) => With((first, second) => first - second, resourceTypes);
    public ResourceTypeStats WithMultiplied(params InMemoryResourceType[] resourceTypes) => With((first, second) => first * second, resourceTypes);
    
    private ResourceTypeStats With(Func<int, int, int> combineOperation, InMemoryResourceType[] resourceTypes)
    {
        foreach (var resourceType in resourceTypes)
            With(resourceType, combineOperation);
        return this;
    }
    
    private ResourceTypeStats With(IResourceType newResource, Func<int, int, int> combineOperation)
    {
        var key = newResource.Name;
        _resources[key] = _resources.TryGetValue(key, out var existing)
            ? new InMemoryResourceType(existing, newResource, combineOperation)
            : new InMemoryResourceType(newResource);
        return this;
    }
    
    public InMemoryResourceType[] AsArray() => _resources.Select(x => x.Value).ToArray();
}

public static class AddedResourceTypeExtensions
{
    public static IStats Plus(this IResourceType first, IResourceType second) 
        => new StatAddends().With(first, second);
}
