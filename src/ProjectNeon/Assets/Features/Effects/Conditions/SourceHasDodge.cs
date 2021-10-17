using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasDodge")]
public class SourceHasDodge : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.Dodge() > 0
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} has no dodge");
    }
}
