using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceWasAtMaxResources")]
public class SourceWasAtMaxResources : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.PrimaryResourceAmount() + ctx.PaidAmount.Amount == ctx.Source.State.PrimaryResource.MaxAmount
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.NameTerm.ToEnglish()} did not have max primary resources.");
    }
}