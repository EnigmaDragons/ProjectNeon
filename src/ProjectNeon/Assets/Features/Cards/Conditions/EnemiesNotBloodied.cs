using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesNotBloodied")]
public class EnemiesNotBloodied : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.GetConsciousEnemies(ctx.Card.Owner).None(x => x.IsBloodied());
    
    public override string Description => "No enemy is bloodied";
}