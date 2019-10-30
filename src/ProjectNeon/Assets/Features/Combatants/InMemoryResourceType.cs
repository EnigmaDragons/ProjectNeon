using UnityEngine;

public sealed class InMemoryResourceType : IResourceType
{
    public string Name { get; set; }
    public Sprite Icon { get; set; }
    public int MaxAmount { get; set; }
    public int StartingAmount { get; set; }
}
