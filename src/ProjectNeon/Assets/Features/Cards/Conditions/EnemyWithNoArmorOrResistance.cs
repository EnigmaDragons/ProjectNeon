using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemyWithNoArmorOrResistance")]
public class EnemyWithNoArmorOrResistance : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.EnemyMembers.Any(x => x.Armor() <= 0 && x.Resistance() <= 0);
}