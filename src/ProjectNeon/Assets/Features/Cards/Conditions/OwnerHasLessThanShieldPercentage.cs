using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasLassThanShieldPercentage")]
public class OwnerHasLessThanShieldPercentage : StaticCardCondition
{
    [SerializeField] private float percent;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.CurrentShield() < ctx.Card.Owner.MaxShield() * percent;
    
    public override string Description => $"I have less than {percent * 100}% of my maximum shields";
}