using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesStunned")]
public class AnyEnemyIsStunned : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.EnemyMembers.Any(x => x.IsStunnedForCard());
}