using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesAreAfflicted")]
public class NoEnemiesAreAfflicted : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(x => x.State.StatusesOfType(StatusTag.DamageOverTime).Length > 0);
}