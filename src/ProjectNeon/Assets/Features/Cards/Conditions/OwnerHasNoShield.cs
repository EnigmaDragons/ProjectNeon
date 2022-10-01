using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasNoShield")]
public class OwnerHasNoShield : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.Shield() == 0;
    
    public override string Description => $"I have no shields";
}
