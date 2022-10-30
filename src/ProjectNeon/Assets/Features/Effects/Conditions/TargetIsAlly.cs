using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsAlly")]
public class TargetIsAlly : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var notAllyMember = ctx.Target.Members.Where(m => m.TeamType != ctx.Source.TeamType);
        return notAllyMember.Any()
            ? new Maybe<string>($"{notAllyMember.EnglishNames()} not Allied with {ctx.Source.NameTerm.ToEnglish()}")
            : Maybe<string>.Missing();
    }
}