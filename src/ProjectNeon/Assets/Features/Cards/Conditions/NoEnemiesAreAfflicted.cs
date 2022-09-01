using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesAreAfflicted")]
public class NoEnemiesAreAfflicted : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(x => x.State.DamageOverTimes().Length > 0);
    
    public override string Description => "No enemy is afflicted";
}