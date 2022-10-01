
public interface CardCondition
{
    bool ConditionMet(CardConditionContext ctx);
    string HighlightMessage { get; }
    string UnhighlightMessage { get; }
}