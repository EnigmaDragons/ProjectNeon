using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasNoResources")]
public class OwnerHasNoResources : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.PrimaryResourceAmount == 0;
    
    public override string Description => $"I have no resources";
}
