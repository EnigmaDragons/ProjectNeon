using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsProminent")]
public class OwnerIsProminent : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.IsProminent();
    
    public override string Description => "Thoughts/Condition041".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition041" };
}