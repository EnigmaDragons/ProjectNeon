using System;
using UnityEngine;

[Serializable]
public sealed class ResourceTypeModifications : IResourceType
{
    [SerializeField] private SimpleResourceType baseResource;
    [SerializeField] private int maxAdjustment;
    [SerializeField] private int startingAdjustment;

    public string Name => baseResource.Name;
    public Sprite Icon => baseResource.Icon;
    public int MaxAmount => maxAdjustment;
    public int StartingAmount => startingAdjustment;
}
