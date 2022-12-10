using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasAllies")]
public class OwnerHasAllies : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AnyAlly(x => x.Id != ctx.Card.Owner.Id);

    public override string Description => $"I have at least 1 ally";
}