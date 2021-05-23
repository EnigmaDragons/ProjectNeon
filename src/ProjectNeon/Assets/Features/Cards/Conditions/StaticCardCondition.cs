using UnityEngine;

public abstract class StaticCardCondition : ScriptableObject, CardCondition
{
    public abstract bool ConditionMet(CardConditionContext ctx);
}