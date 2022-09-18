using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasZeroPrimaryStat")]
public class TargetHasZeroPrimaryStat : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Target.Members.All(m => m.State[m.PrimaryStat()] <= 0)
            ? Maybe<string>.Missing()
            : new Maybe<string>($"Some of the targets have more than 0 primary stat");
    }
}