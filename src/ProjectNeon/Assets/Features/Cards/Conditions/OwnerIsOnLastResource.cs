using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsOnLastResource")]
public class OwnerIsOnLastResource : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.PrimaryResourceAmount == 1;
}