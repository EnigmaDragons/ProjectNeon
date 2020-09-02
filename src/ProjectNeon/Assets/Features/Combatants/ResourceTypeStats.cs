using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceTypeStats
{
    private readonly Dictionary<string, IResourceType> _resources = new Dictionary<string, IResourceType>();

    public ResourceTypeStats WithAdded(params IResourceType[] resourceTypes) => With((first, second) => first + second, resourceTypes);
    public ResourceTypeStats WithMultiplied(params IResourceType[] resourceTypes) => With((first, second) => first * second, resourceTypes);
    
    private ResourceTypeStats With(Func<int, int, int> combineOperation, params IResourceType[] resourceTypes)
    {
        foreach (var resourceType in resourceTypes)
            With(resourceType, combineOperation);
        return this;
    }
    
    private ResourceTypeStats With(IResourceType newResource, Func<int, int, int> combineOperation)
    {
        var key = newResource.Name;
        _resources[key] = _resources.TryGetValue(key, out var existing)
            ? new CombinedResourceTypeStats(existing, newResource, combineOperation)
            : newResource;
        return this;
    }
    
    public IResourceType[] AsArray() => _resources.Select(x => x.Value).ToArray();
    
    private class CombinedResourceTypeStats : IResourceType
    {
        private readonly IResourceType _first;
        private readonly IResourceType _second;
        private readonly Func<int, int, int> _operation;

        public string Name => _first.Name;
        public Sprite Icon => _first.Icon;
        public int MaxAmount => _operation(_first.MaxAmount, _second.MaxAmount);
        public int StartingAmount => _operation(_first.StartingAmount, _second.StartingAmount);

        public CombinedResourceTypeStats(IResourceType first, IResourceType second, Func<int, int, int> operation)
        {
            if (!first.Name.Equals(second.Name))
                throw new ArgumentException("Cannot add {first.Name} to a {second.Name} modifier");
        
            _first = first;
            _second = second;
            _operation = operation;
        }
    }
}

public static class AddedResourceTypeExtensions
{
    public static IStats Plus(this IResourceType first, IResourceType second) 
        => new StatAddends().With(first, second);
}
