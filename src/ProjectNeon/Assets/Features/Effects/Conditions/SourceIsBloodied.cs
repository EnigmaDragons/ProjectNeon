using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceIsBloodied")]
public class SourceIsBloodied : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.IsBloodied()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.NameTerm.ToEnglish()} is not bloodied");
    }
}