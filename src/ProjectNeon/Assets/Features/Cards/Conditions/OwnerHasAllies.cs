using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasAllies")]
public class OwnerHasAllies : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AnyAlly(x => x.Id != ctx.Card.Owner.Id);

    public override string Description => "Thoughts/Condition028".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition028" };
}