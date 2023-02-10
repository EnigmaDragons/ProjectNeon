using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesBloodied")]
public class EnemiesBloodied : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.IsBloodied());
    
    public override string Description => "An enemy is bloodied";
}