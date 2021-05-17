
public interface EffectCondition
{
    Maybe<string> GetShouldNotApplyReason(EffectContext ctx);
}