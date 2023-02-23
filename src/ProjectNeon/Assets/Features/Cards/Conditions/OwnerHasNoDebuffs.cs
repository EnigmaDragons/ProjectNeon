using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasNoDebuffs")]
public class OwnerHasNoDebuffs : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.GetNumDebuffs() == 0;
    
    public override string Description => "Thoughts/Condition034".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition034" };
}