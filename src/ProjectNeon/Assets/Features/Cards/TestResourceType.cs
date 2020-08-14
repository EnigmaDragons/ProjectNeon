using UnityEngine;

public sealed class TestResourceType : ResourceType
{
    public string resourceName = "None";
    public int maxAmount = 0;
    public int startingAmount = 0;

    public override string Name => resourceName;
    public override Sprite Icon { get; }
    public override int MaxAmount => maxAmount;
    public override int StartingAmount => startingAmount;

    public TestResourceType Initialized(string n)
    {
        resourceName = n;
        return this;
    }
}
