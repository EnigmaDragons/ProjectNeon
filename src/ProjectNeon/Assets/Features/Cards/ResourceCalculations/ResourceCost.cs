using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class ResourceCost
{
    [SerializeField] private int cost = 0;
    [SerializeField] private ResourceType resourceType;
    [FormerlySerializedAs("isXCost")] [SerializeField] private bool plusXCost;
    
    public int BaseAmount => cost;
    public ResourceType ResourceType => resourceType;
    public bool PlusXCost => plusXCost;
    
    public ResourceCost() {}

    public ResourceCost(int cost, ResourceType resourceType)
    {
        this.cost = cost;
        this.resourceType = resourceType;
    }
}
