using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasMarks")]
public class TargetHasMarks : StaticEffectCondition
{
    [SerializeField] private int markCount = 1;
    [SerializeField] private bool exactMatch = false;


    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var membersWithoutRequiredMarks = ctx.Target.Members.Where(m => (exactMatch && m.State[TemporalStatType.Marked] == markCount) || (!exactMatch && m.State[TemporalStatType.Marked] >= markCount));
        return membersWithoutRequiredMarks.Any()
            ? new Maybe<string>($"{membersWithoutRequiredMarks.EnglishNames()} do not have {markCount} marks")
            : Maybe<string>.Missing();
    }
}