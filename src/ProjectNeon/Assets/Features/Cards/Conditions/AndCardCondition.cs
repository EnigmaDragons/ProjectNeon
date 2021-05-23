using System.Linq;

public class AndCardCondition : CardCondition
{
    private readonly CardCondition[] _conditions;

    public AndCardCondition(params CardCondition[] conditions) 
        => _conditions = conditions;

    public bool ConditionMet(CardConditionContext ctx) 
        => _conditions.All(t => t.ConditionMet(ctx));
}