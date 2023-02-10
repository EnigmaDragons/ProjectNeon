using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesHaveResources")]
public class AllEnemiesHaveResources : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State.PrimaryResourceAmount >= 1);
    
    public override string Description => "All enemies have resources";
}