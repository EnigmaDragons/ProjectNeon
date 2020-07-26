using UnityEngine;

public sealed class InMemoryResourceType : IResourceType
{
    public string Name { get; set; } = "None";
    public Sprite Icon { get; set; }
    public int MaxAmount { get; set; } = 0;
    public int StartingAmount { get; set; } = 0;
}
