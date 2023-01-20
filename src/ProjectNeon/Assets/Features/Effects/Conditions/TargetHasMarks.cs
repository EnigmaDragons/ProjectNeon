using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasMarks")]
public class TargetHasMarks : StaticEffectCondition
{
    [SerializeField] private int markCount = 1;
    [SerializeField] private bool exactMatch = false;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var markCounts = ctx.Target.Members.Select(m => m.State[TemporalStatType.Marked]);
        var membersWithoutRequiredMarks = ctx.Target.Members.Where(m => 
                (exactMatch && m.State[TemporalStatType.Marked].CeilingInt() != markCount) 
            || (!exactMatch && m.State[TemporalStatType.Marked] < markCount));
        return membersWithoutRequiredMarks.Any()
            ? new Maybe<string>($"{membersWithoutRequiredMarks.EnglishNames()} do not have {markCount} marks. They had [{string.Join(", ", markCounts)}] instead")
            : Maybe<string>.Missing();
    }
}