using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/Unprofitable")]
public class UnprofitableCondition : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.StartingCredits < ctx.CurrentCredits
            ? $"Party is profitable. Gained Credits: {ctx.CurrentCredits - ctx.StartingCredits}"
            : Maybe<string>.Missing();
    }
}