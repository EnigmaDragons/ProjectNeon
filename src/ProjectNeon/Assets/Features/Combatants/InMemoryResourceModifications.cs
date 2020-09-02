
using UnityEngine;

public class InMemoryResourceModifications : IResourceType
{
    public string Name { get; set; }
    public Sprite Icon { get; }
    public int MaxAmount { get; set; }
    public int StartingAmount { get; set; }
}
