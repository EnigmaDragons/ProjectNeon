using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceWasNotAtMaxResources")]
public class SourceWasNotAtMaxResources : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.PrimaryResourceAmount() + ctx.PaidAmount.Amount == ctx.Source.State.PrimaryResource.MaxAmount
            ? new Maybe<string>($"{ctx.Source.NameTerm.ToEnglish()} did have max primary resources.")
            : Maybe<string>.Missing();
    }
}