using System;
using UnityEngine;

[Serializable]
public class SerializableResourceType : IResourceType
{
    public string name = "None";
    public int maxAmount = 0;
    public int startingAmount = 0;

    public string Name => name;
    public Sprite Icon { get; }
    public int MaxAmount => maxAmount;
    public int StartingAmount => startingAmount;
    
    public SerializableResourceType() {}

    public SerializableResourceType(IResourceType r)
    {
        name = r.Name;
        maxAmount = r.MaxAmount;
        startingAmount = r.StartingAmount;
    }
}