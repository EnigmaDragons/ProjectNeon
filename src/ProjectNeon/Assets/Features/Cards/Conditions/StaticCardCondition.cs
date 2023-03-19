using UnityEngine;

public abstract class StaticCardCondition : ScriptableObject, CardCondition, ILocalizeTerms
{
    public abstract bool ConditionMet(CardConditionContext ctx);
    public string HighlightMessage => string.Format("Thoughts/ConditionGoodWithDescription".ToLocalized(), Description);
    public string UnhighlightMessage => string.Format("Thoughts/ConditionBadWithDescription".ToLocalized(), Description);
    public abstract string Description { get; }

    public virtual string[] GetLocalizeTerms() => new[]
    {
        "Thoughts/ConditionGood",
        "Thoughts/CondtionBad",
        "Thoughts/ConditionGoodWithDescription",
        "Thoughts/ConditionBadWithDescription",
    };
}