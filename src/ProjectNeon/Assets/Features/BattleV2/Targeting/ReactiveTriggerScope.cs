using System;

public enum ReactiveTriggerScope
{
    All = 0,
    Self = 1,
    Allies = 2,
    Enemies = 3,
    Originator = 4,
}

public static class ReactiveTriggerScopeExtensions
{
    public static bool IsInTriggerScope(this ReactiveTriggerScope triggerScope, Member originator, Member possessor, Member triggerer)
    {
        if (triggerScope == ReactiveTriggerScope.Self)
            return Equals(possessor, triggerer);
        if (triggerScope == ReactiveTriggerScope.Originator)
            return Equals(originator, triggerer);
        if (triggerScope == ReactiveTriggerScope.Allies)
            return possessor.TeamType == triggerer.TeamType;
        if (triggerScope == ReactiveTriggerScope.Enemies)
            return possessor.TeamType != triggerer.TeamType;
        return true;
    }

    public static ReactiveTriggerScope Parse(string triggerScope) 
        => string.IsNullOrWhiteSpace(triggerScope)
            ? ReactiveTriggerScope.All
            : (ReactiveTriggerScope) Enum.Parse(typeof(ReactiveTriggerScope), triggerScope);
}