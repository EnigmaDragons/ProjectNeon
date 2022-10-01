using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceIsStealthed")]
public class SourceIsStealthed : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Source.IsStealthed()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} is not Stealthed");
    }
}