using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsNotProminent")]
public class OwnerIsNotProminent : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => !ctx.Card.Owner.IsProminent();
    
    public override string Description => "Thoughts/Condition038".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition038" };
}