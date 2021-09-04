using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemyWithNoArmorOrResistance")]
public class EnemyWithNoArmorOrResistance : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.GetConsciousEnemies(ctx.Card.Owner)
            .Any(x => x.Stealth() < 1 && x.Armor() <= 0 && x.Resistance() <= 0);
}