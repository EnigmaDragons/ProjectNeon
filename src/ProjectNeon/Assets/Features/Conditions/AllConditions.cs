using System;
using System.Collections.Generic;
using UnityEngine;

public class AllConditions
{
    private static readonly Dictionary<ActionConditionType, Func<ActionConditionData, Condition>> _createConditionOfType = new Dictionary<ActionConditionType, Func<ActionConditionData, Condition>>
    {
        { ActionConditionType.PerformerHasResource, e => new NoCondition()},
        { ActionConditionType.PerformerHasResource, e => new PerformerHasResourceCondition(e.IntAmount, e.EffectScope, e.ReferencedEffect)}
    };
    
    public static IPayloadProvider Resolve(ActionConditionData conditionData, Member source, Target target, Group group, Scope scope, int amountPaid)
    {
        var condition = Create(conditionData);
        BattleLog.Write($"Checking {conditionData.ConditionType}");
        return condition.Resolve(source, target, group, scope, amountPaid);
    }

    public static Condition Create(ActionConditionData conditionData)
    {
        var condtionType = conditionData.ConditionType;
        if (!_createConditionOfType.ContainsKey(condtionType))
        {
            Debug.LogError($"No EffectType of {condtionType} exists in {nameof(AllEffects)}");
            return _createConditionOfType[ActionConditionType.Nothing](conditionData);
        }
        return _createConditionOfType[condtionType](conditionData);
    }
}