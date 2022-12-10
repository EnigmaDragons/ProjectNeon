using System;

[Serializable]
public class ConditionedCardTagPreference
{
    public CardTag CardTag { get; }
    public StaticCardCondition Condition { get; }
    public int PriorityAdjustment { get; }
}