using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsNotProminent")]
public class OwnerIsNotProminent : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => !ctx.Card.Owner.IsProminent();
}