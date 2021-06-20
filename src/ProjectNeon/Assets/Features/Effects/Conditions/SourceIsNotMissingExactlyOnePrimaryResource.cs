using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceIsNotMissingExactlyOnePrimaryResource")]
public class SourceIsNotMissingExactlyOnePrimaryResource : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.PrimaryResourceAmount() + 1 != ctx.Source.State.PrimaryResource.MaxAmount
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} incorrect primary resource amount");
    }
}
