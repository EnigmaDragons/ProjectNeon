using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesStunned")]
public class EnemiesStunned : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.EnemyMembers.Any(x => x.IsStunnedForCard());
}