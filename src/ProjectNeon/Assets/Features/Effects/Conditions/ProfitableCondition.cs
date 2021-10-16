using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/Profitable")]
public class ProfitableCondition : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.BattleStartingCredits >= ctx.CurrentCredits
            ? $"Party not profitable. Lost Credits: {ctx.CurrentCredits - ctx.BattleStartingCredits}"
            : Maybe<string>.Missing();
    }
}