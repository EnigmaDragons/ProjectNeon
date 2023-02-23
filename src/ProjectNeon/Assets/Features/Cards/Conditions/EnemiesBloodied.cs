using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesBloodied")]
public class EnemiesBloodied : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.IsBloodied());
    
    public override string Description => "Thoughts/Condition012".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition012" };
}