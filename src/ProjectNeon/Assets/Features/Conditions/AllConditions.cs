using System;
using System.Collections.Generic;

public class AllConditions
{
    private static readonly Dictionary<ActionConditionType, Func<ActionConditionData, Condition>> CreateConditionOfType = new Dictionary<ActionConditionType, Func<ActionConditionData, Condition>>
    {
        { ActionConditionType.Nothing, e => new NoCondition()},
        { ActionConditionType.PerformerHasResource, e => new PerformerHasResourceCondition(e.IntAmount, e.EffectScope, e.ReferencedEffect)},
        { ActionConditionType.TargetIsBelowPercentageLife, e => new TargetIsBelowPercentageLifeCondition(e.FloatAmount, e.ReferencedEffect)},
        { ActionConditionType.RepeatForSpent, e => new RepeatForSpentCondition(e.ReferencedEffect) },
        { ActionConditionType.TargetSufferedDamage, e => new TargetSufferedDamageCondition(e.ReferencedEffect) },
    };

    public static IPayloadProvider Resolve(ActionConditionData conditionData, CardActionContext ctx)
    {
        var condition = Create(conditionData);
        Log.Info($"Checking {conditionData.ConditionType}");
        return condition.Resolve(ctx);
    }

    public static Condition Create(ActionConditionData conditionData)
    {
        var conditionType = conditionData.ConditionType;
        if (!CreateConditionOfType.ContainsKey(conditionType))
        {
            Log.Error($"No EffectType of {conditionType} exists in {nameof(AllConditions)}");
            return CreateConditionOfType[ActionConditionType.Nothing](conditionData);
        }
        return CreateConditionOfType[conditionType](conditionData);
    }
}
