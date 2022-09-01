using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesAreDefenseless")]
public class NoEnemiesAreDefenseless : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.Stealth() > 0 || x.Armor() > 0 || x.Resistance() > 0);
    
    public override string Description => "No enemy is defenseless";
}
