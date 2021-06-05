using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsNotBloodied")]
public class TargetIsNotBloodied : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var bloodiedMembers = ctx.Target.Members.Where(m => m.IsBloodied());
        return bloodiedMembers.Any()
            ? new Maybe<string>($"{bloodiedMembers.Names()} bloodied")
            : Maybe<string>.Missing();
    }
}