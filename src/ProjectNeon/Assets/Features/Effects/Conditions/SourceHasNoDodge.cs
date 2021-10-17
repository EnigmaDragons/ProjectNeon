using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasNoDodge")]
public class SourceHasNoDodge : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.Dodge() > 0
            ? new Maybe<string>($"{ctx.Source.Name} has dodge")
            : Maybe<string>.Missing();
    }
}