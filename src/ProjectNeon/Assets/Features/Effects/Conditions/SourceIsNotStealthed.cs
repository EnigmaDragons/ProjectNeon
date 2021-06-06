using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceIsNotStealthed")]
public class SourceIsNotStealthed : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return !ctx.Source.IsStealthed()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} is Stealthed");
    }
}