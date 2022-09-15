using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceHasNoShields")]
public class SourceHasNoShields: StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.CurrentShield() == 0
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} does have shields");
    }
}
