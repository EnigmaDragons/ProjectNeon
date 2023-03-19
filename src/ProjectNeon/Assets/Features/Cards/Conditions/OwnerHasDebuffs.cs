using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasDebuffs")]
public class OwnerHasDebuffs : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.GetNumDebuffs() > 0;
    
    public override string Description => "Thoughts/Condition030".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition030" };
}