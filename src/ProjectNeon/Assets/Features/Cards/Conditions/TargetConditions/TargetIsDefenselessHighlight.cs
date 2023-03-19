using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Defenseless")]
public class TargetIsDefenselessHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(x => x.Armor() <= 0 && x.Resistance() <= 0);
}