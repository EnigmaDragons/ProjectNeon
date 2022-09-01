using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasAtLeastShieldPercentage")]
public class OwnerHasAtLeastShieldPercentage : StaticCardCondition
{
    [SerializeField] private float percent;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.CurrentShield() >= ctx.Card.Owner.MaxShield() * percent;
    
    public override string Description => $"I have at least {percent * 100}% of my maximum shields";
}
