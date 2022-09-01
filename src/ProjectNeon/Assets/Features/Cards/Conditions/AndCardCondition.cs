using System.Linq;

public class AndCardCondition : CardCondition
{
    private readonly CardCondition[] _conditions;

    public AndCardCondition(params CardCondition[] conditions) 
        => _conditions = conditions;

    public bool ConditionMet(CardConditionContext ctx) 
        => _conditions.All(t => t.ConditionMet(ctx));
    
    public string HighlightMessage => _conditions.Length == 1 ? _conditions[0].HighlightMessage : $"This could be good.";
    public string UnhighlightMessage => _conditions.Length == 1 ? _conditions[0].UnhighlightMessage : $"This might be a bad idea.";
}