using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/SimpleResourceType")]
public sealed class SimpleResourceType : ResourceType
{
    [SerializeField] private int maxAmount;
    [SerializeField] [PreviewSprite] private Sprite icon;
    [SerializeField] private int startingAmount;

    public override string Name => name;
    public override Sprite Icon => icon;
    public override int MaxAmount => maxAmount;
    public override int StartingAmount => startingAmount;
}
