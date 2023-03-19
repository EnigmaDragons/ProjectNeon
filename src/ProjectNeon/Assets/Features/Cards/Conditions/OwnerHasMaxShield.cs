using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasMaxShield")]
public class OwnerHasMaxShield: StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.Shield() == ctx.Card.Owner.State.MaxShield();
    
    public override string Description => "Thoughts/Condition033".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition033" };
}
