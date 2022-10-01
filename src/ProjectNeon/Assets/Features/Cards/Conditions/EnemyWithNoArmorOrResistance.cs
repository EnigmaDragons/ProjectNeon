using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemyWithNoArmorOrResistance")]
public class EnemyWithNoArmorOrResistance : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AnyEnemy(x => x.Stealth() < 1 && x.Armor() <= 0 && x.Resistance() <= 0);
    
    public override string Description => "An enemy has no armor or resistance and is defenseless";
}