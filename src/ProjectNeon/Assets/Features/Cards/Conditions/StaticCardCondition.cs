using UnityEngine;

public abstract class StaticCardCondition : ScriptableObject, CardCondition, ILocalizeTerms
{
    public abstract bool ConditionMet(CardConditionContext ctx);
    public string HighlightMessage => "Thoughts/ConditionGoodWithDescription".ToLocalized().SafeFormatWithDefault("This could be good: {0}", Description);
    public string UnhighlightMessage => "Thoughts/ConditionBadWithDescription".ToLocalized().SafeFormatWithDefault("This might be a bad idea: {0}.", Description);
    public abstract string Description { get; }

    public virtual string[] GetLocalizeTerms() => new[]
    {
        "Thoughts/ConditionGood",
        "Thoughts/CondtionBad",
        "Thoughts/ConditionGoodWithDescription",
        "Thoughts/ConditionBadWithDescription",
    };
}