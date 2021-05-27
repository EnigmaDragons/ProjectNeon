using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesNotBloodied")]
public class EnemiesNotBloodied : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.EnemyMembers.None(x => x.IsBloodied());
}