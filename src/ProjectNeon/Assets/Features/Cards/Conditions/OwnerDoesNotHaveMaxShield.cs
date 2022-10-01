using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerDoesNotHaveMaxShield")]
public class OwnerDoesNotHaveMaxShield: StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.Shield() != ctx.Card.Owner.State.MaxShield();
    
    public override string Description => $"I don't have maximum shield";
}
