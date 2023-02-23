using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasNoResources")]
public class OwnerHasNoResources : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.PrimaryResourceAmount == 0;
    
    public override string Description => "Thoughts/Condition035".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition035" };
}
