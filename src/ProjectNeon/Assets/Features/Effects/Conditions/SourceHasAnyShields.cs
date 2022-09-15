using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasAnyShields")]
public class SourceHasAnyShields : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.CurrentShield() > 0
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} does not have any shields");
    }
}
