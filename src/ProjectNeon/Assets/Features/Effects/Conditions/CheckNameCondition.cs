﻿using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/Check Named Condition")]
public class CheckNameCondition : StaticEffectCondition
{
    [SerializeField] private string conditionName;
    [SerializeField] private StaticEffectCondition condition;

    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var chosenName = string.IsNullOrWhiteSpace(conditionName) ? condition.name : conditionName;
        return ctx.ScopedData.IsCondition(chosenName)
            ? $"Did not fulfill precalculated condition: {chosenName}"
            : Maybe<string>.Missing();
    }
}