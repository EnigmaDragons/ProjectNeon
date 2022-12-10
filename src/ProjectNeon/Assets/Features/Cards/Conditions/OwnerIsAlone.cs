using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerIsAlone")]
public class OwnerIsAlone : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoAlly(x => x.Id != ctx.Card.Owner.Id);
    
    public override string Description => $"I am alone";
}
