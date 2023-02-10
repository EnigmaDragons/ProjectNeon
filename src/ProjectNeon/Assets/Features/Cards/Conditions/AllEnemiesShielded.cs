using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesAreShielded")]
public class AllEnemiesShielded : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State[TemporalStatType.Shield] >= 1);
    
    public override string Description => "All enemies are shielded";
}