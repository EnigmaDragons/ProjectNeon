using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasNumberOfPrimaryResources")]
public class SourceHasAmountOfPrimaryResource : StaticEffectCondition
{
    [SerializeField] private int amount;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.PrimaryResourceAmount() == amount
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} does not have {amount} Primary Resources");
    }
}
