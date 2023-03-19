using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/Shields")]
public sealed class TargetHasShieldsHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(x => x.State[TemporalStatType.Shield] > 0);
}