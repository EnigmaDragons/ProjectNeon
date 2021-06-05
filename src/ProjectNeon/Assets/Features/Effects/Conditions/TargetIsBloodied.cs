using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsBloodied")]
public class TargetIsBloodied : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var notBloodiedMembers = ctx.Target.Members.Where(m => !m.IsBloodied());
        return notBloodiedMembers.Any()
            ? new Maybe<string>($"{notBloodiedMembers.Names()} not bloodied")
            : Maybe<string>.Missing();
    }
}