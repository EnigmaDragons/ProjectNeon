using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemyWithNoArmorOrResistance")]
public class EnemyWithNoArmorOrResistance : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.Stealth() < 1 && x.Armor() <= 0 && x.Resistance() <= 0);
    
    public override string Description => "All enemies have no armor or resistance and are defenseless";
}