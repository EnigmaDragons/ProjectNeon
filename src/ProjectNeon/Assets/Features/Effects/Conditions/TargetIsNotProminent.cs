using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetIsNotProminent")]
public class TargetIsNotProminent : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var prominents = ctx.Target.Members.Where(m => m.State[TemporalStatType.Prominent] > 0);
        return prominents.Any()
            ? new Maybe<string>($"{prominents.Names()} was prominent")
            : Maybe<string>.Missing();
    }
}