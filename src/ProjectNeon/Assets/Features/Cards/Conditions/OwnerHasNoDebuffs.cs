using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasNoDebuffs")]
public class OwnerHasNoDebuffs : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.GetNumDebuffs() == 0;
    
    public override string Description => $"I have no debuffs";
}