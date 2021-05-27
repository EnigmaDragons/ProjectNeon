using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsProminent")]
public class OwnerIsProminent : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.IsProminent();
}