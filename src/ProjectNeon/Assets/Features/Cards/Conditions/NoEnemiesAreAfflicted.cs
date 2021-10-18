using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesAreAfflicted")]
public class NoEnemiesAreAfflicted : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.ConsciousEnemyMembers.None(x => x.State.StatusesOfType(StatusTag.DamageOverTime).Length > 0);
}