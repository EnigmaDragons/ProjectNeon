using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemyWithNoArmorOrResistance")]
public class EnemyWithNoArmorOrResistance : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.Stealth() < 1 && x.Armor() <= 0 && x.Resistance() <= 0);
    
    public override string Description => "Thoughts/Condition014".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition014" };
}