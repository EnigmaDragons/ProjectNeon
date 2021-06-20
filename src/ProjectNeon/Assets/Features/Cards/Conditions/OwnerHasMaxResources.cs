using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasMaxResources")]
public class OwnerHasMaxResources : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
      => ctx.Card.Owner.State.PrimaryResourceAmount == ctx.Card.Owner.State.PrimaryResource.MaxAmount;
}
