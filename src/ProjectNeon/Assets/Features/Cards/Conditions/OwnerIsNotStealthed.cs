using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsNotStealthed")]
public class OwnerIsNotStealthed : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx) 
        => !ctx.Card.Owner.IsStealthed();
    
    public override string Description => $"I am not stealthed";
}