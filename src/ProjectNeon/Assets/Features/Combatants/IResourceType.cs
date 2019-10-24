using UnityEngine;

public interface IResourceType
{
    string Name { get; }
    Sprite Icon { get; }
    int MaxAmount { get; }
    int StartingAmount { get; }
}
