using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/Unprofitable")]
public class UnprofitableCondition : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.BattleStartingCredits < ctx.CurrentCredits
            ? $"Party is profitable. Gained Credits: {ctx.CurrentCredits - ctx.BattleStartingCredits}"
            : Maybe<string>.Missing();
    }
}