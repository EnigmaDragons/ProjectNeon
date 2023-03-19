using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsNotStealthed")]
public class OwnerIsNotStealthed : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx) 
        => !ctx.Card.Owner.IsStealthed();
    
    public override string Description => "Thoughts/Condition039".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition039" };
}