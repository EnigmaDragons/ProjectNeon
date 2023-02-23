using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesHaveResources")]
public class AllEnemiesHaveResources : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State.PrimaryResourceAmount >= 1);
    
    public override string Description => "Thoughts/Condition004".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition004" };
}