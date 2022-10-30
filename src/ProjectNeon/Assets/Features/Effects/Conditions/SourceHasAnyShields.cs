using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasAnyShields")]
public class SourceHasAnyShields : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.CurrentShield() > 0
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.NameTerm.ToEnglish()} does not have any shields");
    }
}
