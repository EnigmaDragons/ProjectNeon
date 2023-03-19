using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Bloodied")]
public class TargetIsBloodiedHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(x => x.IsBloodied());
}