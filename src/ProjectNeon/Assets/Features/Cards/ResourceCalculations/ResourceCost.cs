using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class ResourceCost : IResourceAmount
{
    [SerializeField] private int cost = 0;
    [SerializeField] private ResourceType resourceType;
    [FormerlySerializedAs("isXCost")] [SerializeField] private bool plusXCost;
    
    public int BaseAmount => cost;
    public IResourceType ResourceType => resourceType;
    public bool PlusXCost => plusXCost;
    
    public ResourceCost() {}

    public ResourceCost(int cost, ResourceType resourceType)
    {
        this.cost = cost;
        this.resourceType = resourceType;
    }

    public override string ToString()
    {
        var xString = !plusXCost 
            ? "" 
            : "+X";
        
        return $"{cost}{xString}".Replace("0+", "");
    }
}
