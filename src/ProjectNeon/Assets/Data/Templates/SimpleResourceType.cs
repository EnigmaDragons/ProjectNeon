using UnityEngine;

public sealed class SimpleResourceType : ResourceType
{
    [SerializeField] private int maxAmount;

    public override string Name => name;
    public override int MaxAmount => maxAmount;
}
