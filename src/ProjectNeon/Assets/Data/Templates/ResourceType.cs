
using UnityEngine;

public abstract class ResourceType : ScriptableObject, IResourceType
{
    public abstract string Name { get; }
    public abstract int MaxAmount { get; }
}
