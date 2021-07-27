using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SourceIsStealthed")]
public class SourceIsStealthed : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        Log.Info(ctx.Source.Stealth().ToString());
        return ctx.Source.IsStealthed()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"{ctx.Source.Name} is not Stealthed");
    }
}