using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsStealthed")]
public class OwnerIsStealthed : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx) 
        => ctx.Card.Owner.IsStealthed();
    
    public override string Description => $"I am stealthed";
}