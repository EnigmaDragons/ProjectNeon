using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsOnLastResource")]
public class OwnerIsOnLastResource : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.PrimaryResourceAmount == 1;
    
    public override string Description => $"I only have one remaining";
}