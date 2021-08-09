using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsNotAfflicted")]
public class TargetIsNotAfflicted : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var afflictedMembers = ctx.Target.Members.Where(m => m.IsAfflicted());
        return afflictedMembers.Any()
            ? new Maybe<string>($"{afflictedMembers.Names()} are Afflicted")
            : Maybe<string>.Missing();
    }
}