using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceIsOutOfPrimaryResource")]
public class SourceIsOutOfPrimaryResource : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.PrimaryResourceAmount() <= 0
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} still has some Primary Resources");
    }
}
