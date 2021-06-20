using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasAnyPrimaryResources")]
public class SourceHasAnyPrimaryResources : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.PrimaryResourceAmount() > 0
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} does not have any Primary Resources");
    }
}
