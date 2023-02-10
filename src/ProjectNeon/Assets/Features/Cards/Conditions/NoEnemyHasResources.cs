using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemyHasResources")]
public class NoEnemyHasResources : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State.PrimaryResourceAmount == 0);
    
    public override string Description => "No enemy has resources";
}