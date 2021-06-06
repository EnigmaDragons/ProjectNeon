using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/No Grenades")]
public class NoGrenadesCondition : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.State.ResourceAmount("Grenades") > 0
            ? "Have grenades"
            : Maybe<string>.Missing();
    }
}