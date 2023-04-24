using UnityEngine;

public interface IResourceType
{
    string Name { get; }
    int MaxAmount { get; }
    int StartingAmount { get; }
}

