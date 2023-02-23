using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasNoShield")]
public class OwnerHasNoShield : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.Shield() == 0;
    
    public override string Description => "Thoughts/Condition036".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition036" };
}
