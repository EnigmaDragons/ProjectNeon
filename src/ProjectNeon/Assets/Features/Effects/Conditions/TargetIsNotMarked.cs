using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsNotMarked")]
public class TargetIsNotMarked : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var markedMembers = ctx.Target.Members.Where(m => m.IsMarked());
        return markedMembers.Any()
            ? new Maybe<string>($"{markedMembers.Names()} are Marked")
            : Maybe<string>.Missing();
    }
}