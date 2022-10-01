using UnityEngine;

public abstract class StaticCardCondition : ScriptableObject, CardCondition
{
    public abstract bool ConditionMet(CardConditionContext ctx);
    public string HighlightMessage => $"This could be good: {Description}.";
    public string UnhighlightMessage => $"This might be a bad idea: {Description}.";
    public abstract string Description { get; }
}