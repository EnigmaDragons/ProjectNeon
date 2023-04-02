using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceUsedFirstPrimaryResource")]
public class SourceUsedFirstPrimaryResource : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.PrimaryResourceAmount() + 1 == ctx.Source.State.PrimaryResourceMaxAmount
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.NameTerm.ToEnglish()} did not just use first resource.");
    }
}
