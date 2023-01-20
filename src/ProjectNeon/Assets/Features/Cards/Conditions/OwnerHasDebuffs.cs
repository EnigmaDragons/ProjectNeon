using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasDebuffs")]
public class OwnerHasDebuffs : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.GetNumDebuffs() > 0;
    
    public override string Description => $"I have at least 1 debuff";
}