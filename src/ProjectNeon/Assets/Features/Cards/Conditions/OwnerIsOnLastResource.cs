using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsOnLastResource")]
public class OwnerIsOnLastResource : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.PrimaryResourceAmount == 1;
    
    public override string Description => "Thoughts/Condition040".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition040" };
}