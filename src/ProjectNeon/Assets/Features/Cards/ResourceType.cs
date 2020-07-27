using UnityEngine;

public abstract class ResourceType : ScriptableObject, IResourceType
{
    public abstract string Name { get; }
    public abstract Sprite Icon { get; }
    public abstract int MaxAmount { get; }
    public abstract int StartingAmount { get; }
}
