using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesStunned")]
public class AllEnemiesAreStunned : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.IsStunnedForCard());
    
    public override string Description => "All enemies are stunned";
}