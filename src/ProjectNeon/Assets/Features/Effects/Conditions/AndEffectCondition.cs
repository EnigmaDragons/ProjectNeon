using System.Linq;

public class AndEffectCondition : EffectCondition
{
    private readonly EffectCondition[] _conditions;

    public AndEffectCondition(params EffectCondition[] conditions)
    {
        _conditions = conditions.Where(c => c != null).ToArray();
    }

    public Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        foreach (var t in _conditions)
        {
            var reason = t.GetShouldNotApplyReason(ctx);
            if (reason.IsPresent)
                return reason;
        }

        return Maybe<string>.Missing();
    }
}