using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/Check Named Condition")]
public class CheckNameCondition : StaticEffectCondition
{
    [SerializeField] public string conditionName;
    [SerializeField] public StaticEffectCondition condition;

    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var chosenName = string.IsNullOrWhiteSpace(conditionName) ? condition.name : conditionName;
        return ctx.ScopedData.IsCondition(chosenName)
            ? Maybe<string>.Missing()
            : $"Did not fulfill precalculated condition: {chosenName}";
    }
}