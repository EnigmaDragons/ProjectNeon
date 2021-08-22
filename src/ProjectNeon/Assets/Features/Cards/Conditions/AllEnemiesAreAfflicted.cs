using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesAreAfflicted")]
public class AllEnemiesAreAfflicted : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.EnemyMembers.All(x => x.State.StatusesOfType(StatusTag.DamageOverTime).Length > 0);
}
