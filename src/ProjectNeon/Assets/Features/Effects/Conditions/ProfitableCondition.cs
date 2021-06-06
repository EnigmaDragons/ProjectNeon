using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/Profitable")]
public class ProfitableCondition : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.StartingCredits >= ctx.CurrentCredits
            ? $"Party not profitable. Lost Credits: {ctx.CurrentCredits - ctx.StartingCredits}"
            : Maybe<string>.Missing();
    }
}