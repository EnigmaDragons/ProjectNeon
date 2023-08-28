using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasAtLeastNResources")]
public class OwnerDoesNotHaveAtLeastNResources : StaticCardCondition
{
    [SerializeField] private int amount;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.PrimaryResourceAmount() < amount;
    
    public override string Description => "Thoughts/Condition047".ToLocalized().SafeFormatWithDefault("I have less than {0} resources", amount);
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition047" };
}