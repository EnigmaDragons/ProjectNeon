using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceUsedFirstPrimaryResource")]
public class SourceUsedFirstPrimaryResource : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.PrimaryResourceAmount() + 1 == ctx.Source.State.PrimaryResource.MaxAmount
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} did not just use first resource.");
    }
}
