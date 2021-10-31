using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/NotCondition")]
public class NotCondition : StaticEffectCondition
{
    [SerializeField] private StaticEffectCondition inner;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var innerResult = inner.GetShouldNotApplyReason(ctx);
        return innerResult.IsMissing
            ? "Inner Condition was met, so Not Condition not met"
            : Maybe<string>.Missing();
    }
}
