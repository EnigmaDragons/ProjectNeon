using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsBloodied")]
public class TargetIsBloodied : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var notAfflictedMembers = ctx.Target.Members.Where(m => !m.IsBloodied());
        return notAfflictedMembers.Any()
            ? new Maybe<string>($"{notAfflictedMembers.Names()} not bloodied")
            : Maybe<string>.Missing();
    }
}