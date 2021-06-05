using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsMarked")]
public class TargetIsMarked : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var notMarkedMembers = ctx.Target.Members.Where(m => !m.IsMarked());
        return notMarkedMembers.Any()
            ? new Maybe<string>($"{notMarkedMembers.Names()} not Marked")
            : Maybe<string>.Missing();
    }
}