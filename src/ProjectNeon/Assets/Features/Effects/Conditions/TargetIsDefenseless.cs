using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsDefenceless")]
public class TargetIsDefenseless : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
        => ctx.Target.Members.Any(x => x.Armor() > 0 || x.Resistance() > 0)
            ? $"Target has a member with Armor or Resistance"
            : Maybe<string>.Missing();
}