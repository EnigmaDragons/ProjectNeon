using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasMaxResources")]
public class OwnerHasMaxResources : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
      => ctx.Card.Owner.State.PrimaryResourceAmount == ctx.Card.Owner.State.PrimaryResource.MaxAmount;
    
    public override string Description => "Thoughts/Condition032".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition032" };
}
