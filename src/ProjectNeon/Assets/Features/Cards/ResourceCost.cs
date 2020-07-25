using System;
using UnityEngine;

[Serializable]
public sealed class ResourceCost
{
    [SerializeField] private int cost;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private bool isXCost;

    public int Cost => cost;
    public ResourceType ResourceType => resourceType;
    public bool IsXCost => isXCost;
}
