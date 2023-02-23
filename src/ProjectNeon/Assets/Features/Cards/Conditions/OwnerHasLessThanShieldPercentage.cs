using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasLassThanShieldPercentage")]
public class OwnerHasLessThanShieldPercentage : StaticCardCondition
{
    [SerializeField] private float percent;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.CurrentShield() < ctx.Card.Owner.MaxShield() * percent;
    
    public override string Description => string.Format("Thoughts/Condition031".ToLocalized(), percent * 100);
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition031" };
}