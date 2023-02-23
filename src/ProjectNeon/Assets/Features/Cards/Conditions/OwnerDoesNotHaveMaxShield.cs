using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerDoesNotHaveMaxShield")]
public class OwnerDoesNotHaveMaxShield: StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.State.Shield() != ctx.Card.Owner.State.MaxShield();
    
    public override string Description => "Thoughts/Condition027".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition027" };
}
