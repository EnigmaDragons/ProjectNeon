using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesHaveLessResourcesThanYouAfterPlayingThis")]
public class AllEnemiesHaveLessResourcesThanYouAfterPlayingThis : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => ctx.Card.Owner.PrimaryResourceAmount() - ctx.Card.Cost.BaseAmount > x.PrimaryResourceAmount());
    
    public override string Description => "Thoughts/Condition045".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition045" };
}