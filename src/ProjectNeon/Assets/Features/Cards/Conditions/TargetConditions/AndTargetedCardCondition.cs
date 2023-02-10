using System.Linq;

public class AndTargetedCardCondition : TargetedCardCondition
{
    private readonly TargetedCardCondition[] _conditions;

    public AndTargetedCardCondition(params TargetedCardCondition[] conditions) 
        => _conditions = conditions;

    public bool ConditionMet(TargetedCardConditionContext ctx) 
        => _conditions.All(t => t.ConditionMet(ctx));
}