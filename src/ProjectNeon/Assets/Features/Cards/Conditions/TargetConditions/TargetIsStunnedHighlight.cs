using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Stunned")]
public class TargetIsStunnedHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(x => x.IsStunnedForCard());
}