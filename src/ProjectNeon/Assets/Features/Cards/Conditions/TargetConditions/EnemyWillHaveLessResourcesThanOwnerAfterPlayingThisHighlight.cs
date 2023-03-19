using UnityEngine;

[CreateAssetMenu(menuName = "TargetConditions/EnemyWillHaveLessResourcesThanOwnerAfterPlayingThis")]
public class EnemyWillHaveLessResourcesThanOwnerAfterPlayingThisHighlight : StaticTargetedCardCondition
{
    public override bool ConditionMet(TargetedCardConditionContext ctx)
        => inversed != ctx.TargetIs(x => ctx.Card.Owner.PrimaryResourceAmount() - ctx.Card.Cost.BaseAmount > x.PrimaryResourceAmount());
}