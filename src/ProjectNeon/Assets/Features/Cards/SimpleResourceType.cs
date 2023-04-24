using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/SimpleResourceType")]
public sealed class SimpleResourceType : ResourceType
{
    [SerializeField] private int maxAmount;
    [SerializeField] private int startingAmount;

    public override string Name => name;
    public override int MaxAmount => maxAmount;
    public override int StartingAmount => startingAmount;
}
