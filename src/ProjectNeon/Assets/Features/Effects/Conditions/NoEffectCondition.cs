
public class NoEffectCondition : EffectCondition
{
    public Maybe<string> GetShouldNotApplyReason(EffectContext ctx) => Maybe<string>.Missing();
}