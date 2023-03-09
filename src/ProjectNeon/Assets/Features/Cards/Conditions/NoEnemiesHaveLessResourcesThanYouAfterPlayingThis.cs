using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesHaveLessResourcesThanYouAfterPlayingThis")]
public class NoEnemiesHaveLessResourcesThanYouAfterPlayingThis : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(x => ctx.Card.Owner.PrimaryResourceAmount() - ctx.Card.Cost.BaseAmount > x.PrimaryResourceAmount());
    
    public override string Description => "Thoughts/Condition046".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition046" };
}