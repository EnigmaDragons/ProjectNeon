using System;
using System.Collections.Generic;
using TMPro;

public class AllConditions
{
    private static BattleStateSnapshot _beforeCard;
    private static readonly Dictionary<ActionConditionType, Func<ActionConditionData, Condition>> _createConditionOfType = new Dictionary<ActionConditionType, Func<ActionConditionData, Condition>>
    {
        { ActionConditionType.Nothing, e => new NoCondition()},
        { ActionConditionType.PerformerHasResource, e => new PerformerHasResourceCondition(e.IntAmount, e.EffectScope, e.ReferencedEffect)},
        { ActionConditionType.TargetIsBelowPercentageLife, e => new TargetIsBelowPercentageLifeCondition(e.FloatAmount, e.ReferencedEffect)},
        { ActionConditionType.RepeatForSpent, e => new RepeatForSpentCondition(e.ReferencedEffect) },
        { ActionConditionType.TargetSufferedDamage, e => new TargetSufferedDamageCondition(e.ReferencedEffect) },
    };
    
    public static IPayloadProvider Resolve(ActionConditionData conditionData, Member source, Target target, Group group, Scope scope, int amountPaid)
    {
        var condition = Create(conditionData);
        Log.Info($"Checking {conditionData.ConditionType}");
        return condition.Resolve(_beforeCard, source, target, group, scope, amountPaid);
    }

    public static Condition Create(ActionConditionData conditionData)
    {
        var condtionType = conditionData.ConditionType;
        if (!_createConditionOfType.ContainsKey(condtionType))
        {
            Log.Error($"No EffectType of {condtionType} exists in {nameof(AllConditions)}");
            return _createConditionOfType[ActionConditionType.Nothing](conditionData);
        }
        return _createConditionOfType[condtionType](conditionData);
    }

    public static void InitCardPlaying(BattleStateSnapshot beforeCard) => _beforeCard = beforeCard;
}
