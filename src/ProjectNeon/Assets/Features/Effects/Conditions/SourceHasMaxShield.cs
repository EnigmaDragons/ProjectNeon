using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasMaxShield")]
public class SourceHasMaxShield : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.CurrentShield() == ctx.Source.MaxShield()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.NameTerm.ToEnglish()} does not have max shield");
    }
}
