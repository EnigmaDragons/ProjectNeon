using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/Check Named Condition Unmet")]
public class CheckNamedConditionUnmet : StaticEffectCondition
{
    [SerializeField] private string conditionName;
    [SerializeField] private StaticEffectCondition condition;

    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var chosenName = string.IsNullOrWhiteSpace(conditionName) ? condition.name : conditionName;
        return ctx.ScopedData.IsCondition(chosenName)
            ? Maybe<string>.Missing()
            : $"Did fulfill precalculated condition: {chosenName}";
    }
}