using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasShields")]
public class TargetHasShields : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Target.Members.All(m => m.CurrentShield() > 0)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"Some of the targets have no shields");
    }
}