using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Afflicted")]
public sealed class TargetIsAfflictedHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(x => x.IsAfflicted());
}