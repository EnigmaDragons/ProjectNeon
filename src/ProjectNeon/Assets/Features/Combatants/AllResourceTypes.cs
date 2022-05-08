using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Resource Types")]
public class AllResourceTypes : ScriptableObject
{
    private Dictionary<string, ResourceType> _resourceTypes;
    [UnityEngine.UI.Extensions.ReadOnly] public SimpleResourceType[] ResourceTypes; //Unity Collection Readonly

    public Dictionary<string, ResourceType> GetMap() => _resourceTypes ??= ResourceTypes.ToDictionary(x => x.Name, x => (ResourceType)x);
    public Maybe<ResourceType> GetResourceTypeByName(string resourceName) => GetMap().ValueOrMaybe(resourceName);
}
