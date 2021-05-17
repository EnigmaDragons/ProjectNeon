using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsAfflicted")]
public class TargetIsAfflicted : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var notAfflictedMembers = ctx.Target.Members.Where(m => !m.IsAfflicted());
        return notAfflictedMembers.Any()
            ? new Maybe<string>($"{notAfflictedMembers.Names()} not Afflicted")
            : Maybe<string>.Missing();
    }
}
