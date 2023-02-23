using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesNotBloodied")]
public class EnemiesNotBloodied : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.GetConsciousEnemies(ctx.Card.Owner).None(x => x.IsBloodied());
    
    public override string Description => "Thoughts/Condition013".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition013" };
}