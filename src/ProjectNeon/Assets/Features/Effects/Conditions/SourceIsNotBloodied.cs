using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceIsNotBloodied")]
public class SourceIsNotBloodied : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return !ctx.Source.IsBloodied()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} is bloodied");
    }
}