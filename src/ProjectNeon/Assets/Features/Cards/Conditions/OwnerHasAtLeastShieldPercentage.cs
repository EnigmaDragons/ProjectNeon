using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasAtLeastShieldPercentage")]
public class OwnerHasAtLeastShieldPercentage : StaticCardCondition
{
    [SerializeField] private float percent;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.CurrentShield() >= ctx.Card.Owner.MaxShield() * percent;
    
    public override string Description => "Thoughts/Condition029".ToLocalized().SafeFormatWithDefault("I have at least {0}% of my maximum shields", percent * 100);
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition029" };
}
