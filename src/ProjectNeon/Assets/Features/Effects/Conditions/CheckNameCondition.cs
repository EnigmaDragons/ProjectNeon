using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/Check Named Condition")]
public class CheckNameCondition : StaticEffectCondition
{
    [SerializeField] private string _conditionName;

    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.ScopedData.IsCondition(_conditionName)
            ? $"Did not fulfill precalculated condition: {_conditionName}"
            : Maybe<string>.Missing();
    }
}