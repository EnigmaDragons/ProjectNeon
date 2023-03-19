using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Stealthed")]
public class OwnerIsStealthedHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.Card.Owner.IsStealthed();
}