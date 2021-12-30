using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesBloodied")]
public class EnemiesBloodied : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AnyEnemy(x => x.IsBloodied());
}