using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesHaveShields")]
public class NoEnemiesHaveShields : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State[TemporalStatType.Shield] == 0);
    
    public override string Description => "No enemies have shields";
}