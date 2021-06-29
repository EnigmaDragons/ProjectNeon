using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasNoShields")]
public class TargetHasNoShields : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Target.Members.All(m => m.CurrentShield() <= 0)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"Some of the targets have shields");
    }
}